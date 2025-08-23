using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.DTO;
using HallBookingBhatPara.Domain.DTO.HallBooking;
using HallBookingBhatPara.Infrastructure.Service;
using HallBookingBhatPara.Model.Validator;
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

            model.userClaims = _tokenProvider.GetUserClaims();

            model.EntryIP = _tokenProvider.GetClientIpAddress(HttpContext);



            var response = await _unitOfWork.SPRepository.BookUserConfirmedHallAsync(model);

            if (response == 0)
            {
                return Json(ResponseService.InternalServerResponse<string>("Failed."));
            }

            return Json(ResponseService.SuccessResponse<string>("Successfully"));

        }

        public async Task<IActionResult> GetPaymentSummeryDetails(long selectedPaymentType, long AvailId)
        {
            if (selectedPaymentType == 0 || AvailId == 0)
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


            var paymentSummery = await _unitOfWork.SPRepository.GetPaymentSummeryDetailsAsync(AvailId, 2, PercentageOfIntialPaymentAmount);
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
        #endregion


    }
}
