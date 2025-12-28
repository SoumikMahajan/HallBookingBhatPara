using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Application.Interface.Payments;
using HallBookingBhatPara.Domain.DTO;
using HallBookingBhatPara.Domain.DTO.CashFreePayment;
using HallBookingBhatPara.Domain.DTO.HallBooking;
using HallBookingBhatPara.Infrastructure.Repository;
using HallBookingBhatPara.Infrastructure.Service;
using HallBookingBhatPara.Model.Validator;
using HallBookingBhatPara.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HallBookingBhatPara.Controllers
{
	//[Authorize(Roles = "Public User,Dev")]
	[Authorize]
	public class UserBookingController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenProvider _tokenProvider;
		private readonly ICashfreeService _cashfreeService;
        private readonly LogService _logService;

		public UserBookingController(IUnitOfWork unitOfWork, ITokenProvider tokenProvider, ICashfreeService cashfreeService, LogService logService)
        {
            _unitOfWork = unitOfWork;
            _tokenProvider = tokenProvider;
			_cashfreeService = cashfreeService;
            _logService = logService;
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


			List<UserListDTO> userDdlList = new ();

			var loggedInRoleId = Convert.ToInt64(_tokenProvider.GetUserClaims().RolesId);

			//get user list
			if (loggedInRoleId == 5 || loggedInRoleId == 4)
			{
				userDdlList = await _unitOfWork.SPRepository.GetAllUserListForHallBooking();
				mm.userListDTOs = userDdlList;
			}
						
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



            //double remainingAmount = 0;
            //if (model.OnloadPaymentTypeId == 2 && model.SelectedPaymentTypeId == 20)
            //{
            //    remainingAmount = (paymentSummery.rate * dateCount) - ((paymentSummery.rate * dateCount) * 0.75);
            //}

            model.PaymentSummeryDTO = paymentSummery;
            model.userClaims = _tokenProvider.GetUserClaims();
            model.EntryIP = _tokenProvider.GetClientIpAddress(HttpContext);

            var bookingIdList = (await _unitOfWork.HallBookingDetailsRepository
                            .GetAllAsync())
                            .Select(b => b.booking_reference_id)
                            .ToHashSet();

            // Ensure the generated booking ID is unique
            string bookingId = GenerateUniqueBookingIdAsync(bookingIdList);

			var response = await _unitOfWork.SPRepository.BookUserConfirmedHallAsync(model, dateCount, paymentSummery.TotalPriceSummaryAmount, bookingId);

			if (response == 0)
			{
				return Json(ResponseService.InternalServerResponse<string>("Failed."));
			}

			return Json(ResponseService.SuccessResponse<string>(
				$"Your hall has been booked successfully!<br><br>" +
				$"<strong>Booking ID:</strong> {bookingId}<br><br>" +
				$"Please wait for final approval from the admin. You will be notified once your booking is confirmed."
			));

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

		public async Task<IActionResult> GetUserDetailsOnHallBookingByUserId(long UserId)
		{
			if (UserId <= 0)
			{
				return Json(ResponseService.BadRequestResponse<string>("UserId can not null or empty or 0"));
			}
			var UserList = await _unitOfWork.UserRegistrationRepository.GetAsync(c => c.user_id_pk == UserId);
			if (UserList == null)
			{
				return Json(ResponseService.NotFoundResponse<string>("No User Found."));
			}

			return Json(ResponseService.SuccessResponse(UserList));
		}


		#region :: Submit For Counter Admin
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> BookUserConfirmedHallForCounterAdmin([FromForm] InsertUserConfirmhallForCounterAdminDTO model)
		{
			var validator = new InsertConfirmBookHallForCounterAdminValidator();
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

			var response = await _unitOfWork.SPRepository.BookUserConfirmedHallForCounterAdminAsync(model, percentage, remainingAmount, dateCount, paymentSummery.TotalPriceSummaryAmount, bookingId);

			if (response == 0)
			{
				return Json(ResponseService.InternalServerResponse<string>("Failed."));
			}

			return Json(ResponseService.SuccessResponse<string>("Success."));

		}
		#endregion

		#endregion


		#region :: Booking Details
		public IActionResult BookingList()
        {
            return View();
        }

        public async Task<IActionResult> UserHallBookedList()
        {
			var loggedInRoleId = Convert.ToInt64(_tokenProvider.GetUserClaims().RolesId);
			var loggedInStackId = Convert.ToInt64(_tokenProvider.GetUserClaims().StackHolderId);

			MultipleModel mm = new();
            List<BookedListDTO> hallBookedList = new();

            var response = await _unitOfWork.SPRepository.PublicUserHallBookedDetailsAsync(loggedInRoleId, loggedInStackId);

            mm.bookedListDTOs = response ?? new List<BookedListDTO>();

            return PartialView("_partialUserHallBookedList", mm);
        }
		#endregion


		#region :: CashFree Payment Intregation

		//[HttpGet]
		//public async Task<IActionResult> PaymentCallback(string order_id)
		//{
		//	try
		//	{

		//		if (string.IsNullOrEmpty(order_id))
		//		{
		//			return View("PaymentFailed");
		//		}

		//		// Fetch order status from Cashfree
		//		var orderStatus = await _cashfreeService.GetOrderStatusAsync(order_id);
		//		var paymentDetails = await _cashfreeService.GetPaymentDetailsAsync(order_id);

		//		if (orderStatus != null)
		//		{
		//			// Check payment status
		//			if (orderStatus.OrderStatus == "PAID")
		//			{
		//				// Payment successful - Update your database here
		//				ViewBag.OrderId = order_id;
		//				ViewBag.Amount = orderStatus.OrderAmount;
		//				ViewBag.Status = "Success";
		//				return View("PaymentSuccess");
		//			}
		//			else if (orderStatus.OrderStatus == "ACTIVE")
		//			{
		//				// Payment is still pending
		//				ViewBag.Message = "Payment is being processed";
		//				return View("PaymentPending");
		//			}
		//		}

		//		// Payment failed or other status
		//		ViewBag.OrderId = order_id;
		//		ViewBag.Status = orderStatus?.OrderStatus ?? "Unknown";
		//		return View("PaymentFailed");
		//	}
		//	catch (Exception)
		//	{				
		//		return View("PaymentFailed");
		//	}
		//}

		private async Task<CreateOrderResponse?> CreateCashfreeOrderAsync(string orderId)
		{
			var customerId = $"cust_{DateTime.UtcNow.Ticks}";

			var orderRequest = new CreateOrderRequest
			{
				OrderId = orderId,
				OrderCurrency = "INR",
				OrderAmount = 100,
				CustomerDetails = new CustomerDetails
				{
					CustomerId = customerId,
					CustomerPhone = "1234567890",
					CustomerEmail = "sample@yopmail.com",
					CustomerName = "Sample Test"
				},
				OrderNote = $"Payment for Order {orderId}",
				OrderMeta = new OrderMeta
				{
					ReturnUrl = Url.Action("PaymentCallback", "UserBooking", null, Request.Scheme) + "?order_id={order_id}"
					// NotifyUrl, PaymentMethods etc. can be added here if needed
				}
			};

			var responce =  await _cashfreeService.CreateOrderAsync(orderRequest);

            return responce;
		}

		[HttpGet]
		public async Task<IActionResult> PaymentCallback(string order_id)
		{
			try
			{
				if (string.IsNullOrEmpty(order_id))
				{
					await _logService.LogCustomAsync("PaymentCallback called without order_id");
					return View("PaymentFailed", new PaymentResultViewModel
					{
						ErrorMessage = "Invalid payment reference"
					});
				}

				// Fetch order status from Cashfree
				var orderStatus = await _cashfreeService.GetOrderStatusAsync(order_id);

				if (orderStatus == null)
				{
					await _logService.LogCustomAsync($"Failed to fetch order status from Cashfree. OrderId: {order_id}");
					return View("PaymentFailed", new PaymentResultViewModel
					{
						OrderId = order_id,
						ErrorMessage = "Unable to verify payment status"
					});
				}

				// Verify payment details
				var paymentDetails = await _cashfreeService.GetPaymentDetailsAsync(order_id);

				// CRITICAL: Verify the payment amount matches your database record
				//var bookingRecord = await _bookingService.GetBookingByOrderIdAsync(order_id);
				//if (bookingRecord == null)
				//{
				//	_logger.LogError("Booking not found for OrderId: {OrderId}", order_id);
				//	return View("PaymentFailed", new PaymentResultViewModel
				//	{
				//		OrderId = order_id,
				//		ErrorMessage = "Booking reference not found"
				//	});
				//}

				// Verify amount to prevent tampering
				//if (orderStatus.OrderAmount != bookingRecord.Amount)
				//{
				//	_logger.LogError("Amount mismatch. Expected: {Expected}, Received: {Received}, OrderId: {OrderId}",
				//		bookingRecord.Amount, orderStatus.OrderAmount, order_id);

				//	return View("PaymentFailed", new PaymentResultViewModel
				//	{
				//		OrderId = order_id,
				//		ErrorMessage = "Payment amount verification failed"
				//	});
				//}

				switch (orderStatus.OrderStatus)
				{
					case "PAID":
						// Payment successful - Update database with transaction lock
						//var updateResult = await _bookingService.UpdatePaymentStatusAsync(
						//	orderId: order_id,
						//	status: "PAID",
						//	transactionId: paymentDetails?.FirstOrDefault()?.CfPaymentId,
						//	paymentMethod: paymentDetails?.FirstOrDefault()?.PaymentGroup,
						//	paidAmount: orderStatus.OrderAmount
						//);

						//if (!updateResult)
						//{
						//	await _logService.LogCustomAsync($"Database update failed for paid order: {order_id}");
						//	// Send alert to admin - payment received but DB update failed
						//}

						// Send confirmation email/SMS
						//await SendBookingConfirmationAsync(bookingRecord);

						return View("PaymentSuccess", new PaymentResultViewModel
						{
							OrderId = order_id,
							Amount = orderStatus.OrderAmount,
							Status = "Success",
							BookingId = "",
							TransactionId = paymentDetails?.FirstOrDefault()?.CfPaymentId
						});

					case "ACTIVE":
						// Payment is still pending
						await _logService.LogCustomAsync($"Payment pending for OrderId: {order_id}");
						return View("PaymentPending", new PaymentResultViewModel
						{
							OrderId = order_id,
							Message = "Your payment is being processed. You will receive confirmation shortly."
						});

					case "EXPIRED":
						//await _bookingService.UpdatePaymentStatusAsync(order_id, "EXPIRED");
						return View("PaymentFailed", new PaymentResultViewModel
						{
							OrderId = order_id,
							Status = "Expired",
							ErrorMessage = "Payment session expired. Please try booking again."
						});

					default:
						// Payment failed or cancelled
						await _logService.LogCustomAsync($"Payment failed/cancelled. OrderId: {order_id}, Status: {orderStatus.OrderStatus}");

						//await _bookingService.UpdatePaymentStatusAsync(order_id, orderStatus.OrderStatus);

						return View("PaymentFailed", new PaymentResultViewModel
						{
							OrderId = order_id,
							Status = orderStatus.OrderStatus,
							ErrorMessage = GetUserFriendlyErrorMessage(orderStatus.OrderStatus)
						});
				}

			}
			catch (Exception)
			{
				await _logService.LogCustomAsync($"Exception in PaymentCallback. OrderId: {order_id}");
				return View("PaymentFailed", new PaymentResultViewModel
				{
					OrderId = order_id,
					ErrorMessage = "An error occurred while processing your payment. Please contact support with your order reference."
				});
			}
		}

		private string GetUserFriendlyErrorMessage(string status)
		{
			return status switch
			{
				"USER_DROPPED" => "Payment was cancelled by you",
				"VOID" => "Payment was cancelled",
				"TERMINATED" => "Payment session terminated",
				_ => "Payment could not be completed. Please try again."
			};
		}

		#endregion


	}
}
