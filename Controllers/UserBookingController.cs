using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.DTO;
using HallBookingBhatPara.Domain.DTO.HallBooking;
using HallBookingBhatPara.Infrastructure.Service;
using HallBookingBhatPara.Model.Validator;
using HallBookingBhatPara.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallBookingBhatPara.Controllers
{
    [Authorize(Roles = "Public User,Dev")]
    public class UserBookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenProvider _tokenProvider;

        public UserBookingController(IUnitOfWork unitOfWork, ITokenProvider tokenProvider)
        {
            _unitOfWork = unitOfWork;
            _tokenProvider = tokenProvider;
        }

        #region :: Hall Search
        public async Task<IActionResult> UserHallBooking()
        {
            MultipleModel mm = new();
            var dropDownList = (await _unitOfWork.CategoryMasterRepository.GetAllAsync(c => c.active_status == 1))
                             .Select(c => new DropDownListDTO { Id = c.category_id_pk, Name = c.category_name })
                             .ToList();

            mm.dropDownListDTOs = dropDownList;
            return View(mm);
        }

        public async Task<IActionResult> HallAvailableSearchResult(long catType, string startDate, string endDate)
        {
            MultipleModel mm = new();
            List<HallSearchDTO> hallSearchList = new();

            var response = await _unitOfWork.SPRepository.HallAvailableSearchResultAsync(catType, startDate, endDate);

            mm.hallSearchList = response ?? new List<HallSearchDTO>();

            return PartialView("_partialHallAvailableSearchResult", mm);
        }
        #endregion

        #region :: User Hall Book
        public async Task<IActionResult> HallDetailsBooking(long hallAvlId)
        {
            MultipleModel mm = new();

            var LoginUserMail = _tokenProvider.GetUserClaims().Email;

            var UserRegDeatils = await _unitOfWork.UserRegistrationRepository.GetAsync(filter: u => u.email == LoginUserMail && u.active_status == 1);
            mm.public_User_Registration = UserRegDeatils;

            var hallDetails = await _unitOfWork.SPRepository.GetHallDetailsAfterSearchAsync(hallAvlId);

            mm.hallBookingDTO = hallDetails;

            var drpDownHallEvent = (await _unitOfWork.HallEventMasterRepository.GetAllAsync(h => h.active_status == 1))
                .Select(h => new DropDownListDTO { Id = h.hall_event_type_id_pk, Name = h.event_type_name })
                .ToList();

            //var FloorList = await _unitOfWork.SPRepository.GetFloorListBySubCatIdAsync(hallDetails.hall_id_pk);

            mm.dropDownListDTOs = drpDownHallEvent;
            //mm.FloorList = FloorList;

            var PercentageOfIntialPaymentAmount = 0;
            if (hallDetails.payment_type_id_fk == 1) // Full Payment
            {
                PercentageOfIntialPaymentAmount = 100;
            }
            else if (hallDetails.payment_type_id_fk == 2) // Half Payment
            {
                PercentageOfIntialPaymentAmount = 75;
            }

            var paymentSummery = await _unitOfWork.SPRepository.GetPaymentSummeryDetailsAsync(hallDetails.hall_availability_id_pk, hallDetails.payment_type_id_fk, PercentageOfIntialPaymentAmount);
            mm.paymentSummeryDTO = paymentSummery;

            return View(mm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BookUserConfirmedHall([FromForm] InsertUserConfirmhallDTO model)
        {
            var validator = new InsertConfirmBookHallValidator();
            var validationResult = validator.Validate(model);

            if (!validationResult.IsValid)
            {
                return Json(ResponseService.FluentValidationErrorResponse<object>(validationResult.Errors));
            }


            //Add validation for checking the event date already booked or not
            var isDateBooked = await _unitOfWork.SPRepository.IsEventDateAlreadyBookedAsync(model.hallAvailId, model.eventDate);
            if (isDateBooked > 0)
            {
                return Json(ResponseService.BadRequestResponse<string>("The selected event date is already booked. Please choose a different date."));
            }

            var percentage = GetPaymentPercentage(model.OnloadPaymentTypeId, model.SelectedPaymentTypeId);


            var dateCount = model.eventDate.Split('^').Length;

            var paymentSummery = await _unitOfWork.SPRepository.GetPaymentSummeryDetailsForMultipleDateAsync(model.hallAvailId, model.OnloadPaymentTypeId, percentage, dateCount);
            if (paymentSummery == null)
                return Json(ResponseService.BadRequestResponse<string>("paymentSummery can not be null"));
            if (paymentSummery.rate == 0)
                return Json(ResponseService.BadRequestResponse<string>("Rate can not be 0"));
            if (paymentSummery.payable_amount == 0)
                return Json(ResponseService.BadRequestResponse<string>("Payable_amount can not be 0"));



            double remainingAmount = 0;
            if (model.OnloadPaymentTypeId == 2 && model.SelectedPaymentTypeId == 20)
            {
                remainingAmount = (paymentSummery.rate * dateCount) - ((paymentSummery.rate * dateCount) * 0.75);
            }

            model.PaymentSummeryDTO = paymentSummery;
            model.userClaims = _tokenProvider.GetUserClaims();
            model.EntryIP = _tokenProvider.GetClientIpAddress(HttpContext);

            var bookingIdList = (await _unitOfWork.HallBookingDetailsRepository
                            .GetAllAsync())
                            .Select(b => b.booking_reference_id)
                            .ToHashSet();

            // Ensure the generated booking ID is unique
            string bookingId = GenerateUniqueBookingIdAsync(bookingIdList);

            var response = await _unitOfWork.SPRepository.BookUserConfirmedHallAsync(model, percentage, remainingAmount, dateCount, paymentSummery.TotalPriceSummaryAmount, bookingId);

            if (response == 0)
            {
                return Json(ResponseService.InternalServerResponse<string>("Failed."));
            }

            //string bookingId = BookingIdGenerator.Generate();




            return Json(ResponseService.SuccessResponse<string>("Successfully"));

        }

        public string GenerateUniqueBookingIdAsync(ISet<string> existingIds)
        {
            string bookingId;
            do
            {
                bookingId = BookingIdGenerator.Generate();
            }
            while (existingIds.Contains(bookingId)); // regenerate if duplicate

            return bookingId;
        }

        private int GetPaymentPercentage(long onloadPaymentTypeId, long selectedPaymentTypeId)
        {
            if (onloadPaymentTypeId == 1) return 100;
            if (onloadPaymentTypeId == 2)
            {
                return selectedPaymentTypeId == 10 ? 100 : 75;
            }
            return 0;
        }

        public async Task<IActionResult> GetPaymentSummeryDetails(long selectedPaymentType, long AvailId, int TotalDaysCount)
        {
            if (selectedPaymentType == 0 || AvailId == 0 || TotalDaysCount == 0)
            {
                return Json(ResponseService.BadRequestResponse<string>("Invalid Data."));
            }

            MultipleModel mm = new();
            var PercentageOfIntialPaymentAmount = 0;
            if (selectedPaymentType == 10) // Full Payment
            {
                PercentageOfIntialPaymentAmount = 100;
            }
            else if (selectedPaymentType == 20) // Half Payment
            {
                PercentageOfIntialPaymentAmount = 75;
            }


            //var paymentSummery = await _unitOfWork.SPRepository.GetPaymentSummeryDetailsAsync(AvailId, 2, PercentageOfIntialPaymentAmount);
            var paymentSummery = await _unitOfWork.SPRepository.GetPaymentSummeryDetailsForMultipleDateAsync(AvailId, 2, PercentageOfIntialPaymentAmount, TotalDaysCount);
            mm.paymentSummeryDTO = paymentSummery;
            ViewBag.NewPaymentId = selectedPaymentType;

            return PartialView("_partialPriceSummary", mm);
        }
        #endregion


        #region :: Booking Details
        public IActionResult BookingList()
        {
            return View();
        }

        public async Task<IActionResult> UserHallBookedList()
        {
            var UserId = Convert.ToInt64(_tokenProvider.GetUserClaims().Id);

            MultipleModel mm = new();
            List<BookedListDTO> hallBookedList = new();

            var response = await _unitOfWork.SPRepository.UserHallBookedDetailsAsync(UserId);

            mm.bookedListDTOs = response ?? new List<BookedListDTO>();

            return PartialView("_partialUserHallBookedList", mm);
        }
        #endregion


    }
}
