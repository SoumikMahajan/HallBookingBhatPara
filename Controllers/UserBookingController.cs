using Azure;
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
using System.Globalization;
using static System.Net.WebRequestMethods;

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

			return Json(ResponseService.SuccessResponse<string>(
				$"Hall has been booked successfully!<br><br>" +
				$"<strong>Booking ID:</strong> {bookingId}<br><br>" +
				$"Please wait for final approval from the admin. You will be notified once your booking is confirmed."
			));

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

            var response = await _unitOfWork.SPRepository.PublicUserHallBookedDetailsAsync(loggedInRoleId, loggedInStackId);

            mm.bookedListDTOs = response ?? new List<BookedListDTO>();

            return PartialView("_partialUserHallBookedList", mm);
        }

		public async Task<IActionResult> UserBookingDetailsById(long HallId)
		{
			if (HallId <= 0)
			{
				return Json(ResponseService.BadRequestResponse<string>("HallId can not be null or empty"));
			}
			MultipleModel mm = new();

			var response = await _unitOfWork.SPRepository.BookingDetailsByIdAsync(HallId);
			mm.bookedDetailsById = response;

			return PartialView("_partialHallBookedDetailsById", mm);

		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> UserHallBookingPayment(long HallBookingId,string HallReferenceId)
		{
			if (HallBookingId <= 0)
				return Json(ResponseService.BadRequestResponse<string>("Booking Id can not be Empty"));
			if (string.IsNullOrEmpty(HallReferenceId))
				return Json(ResponseService.BadRequestResponse<string>("Booking Id can not be Empty"));

			//other code to get and save Payment 

			//Cashfree Payment Integration			
			var orderResponse = await CreateCashfreeOrderAsync(orderId: HallReferenceId);

			if (orderResponse != null && !string.IsNullOrEmpty(orderResponse.PaymentSessionId))
			{
				// Log order creation for audit trail
				await _logService.LogCustomAsync($"Cashfree order created. OrderId: {orderResponse.OrderId}, SessionId: {orderResponse.PaymentSessionId}");

				

				return Json(ResponseService.SuccessResponse(orderResponse));
			}

			// Rollback database changes if order creation fails
			return Json(ResponseService.ErrorResponse<string>("Failed to payment. Please try again."));
		}

		#endregion


		#region :: CashFree Payment Intregation		

		private async Task<CreateOrderResponse?> CreateCashfreeOrderAsync(string orderId)
		{

			var orderDetails = await _unitOfWork.SPRepository.GetBookingDetailsByReferenceIdAsync(orderId);

			if (orderDetails.Result <= 0)
			{
				await _logService.LogCustomAsync(orderDetails.message);
				return null;
			}			

			// Check if a valid Cashfree order already exists in your DB
			var existingOrder = await _unitOfWork.SPRepository.GetActiveCashfreeOrderAsync(orderId);
			if (existingOrder.Item3 != null && existingOrder.Item3.OrderStatus == "ACTIVE")
			{
				await _logService.LogCustomAsync(
						$"Reusing existing active Cashfree order. " +
						$"OrderId: {orderId}, " +
						$"CfOrderId: {existingOrder.Item3.CfOrderId}, " +
						$"Expires: {existingOrder.Item3.OrderExpiryTime}");

				return new CreateOrderResponse
				{
					CfOrderId = existingOrder.Item3.CfOrderId,
					OrderId = existingOrder.Item3.OrderId,
					PaymentSessionId = existingOrder.Item3.PaymentSessionId,
					OrderStatus = existingOrder.Item3.OrderStatus,
					OrderAmount = existingOrder.Item3.OrderAmount,
					OrderCurrency = existingOrder.Item3.OrderCurrency,
					OrderExpiryTime = existingOrder.Item3.OrderExpiryTime,
					CreatedTime = existingOrder.Item3.CreatedAt
				};
			}

			//var customerId = $"cust_{DateTime.UtcNow.Ticks}";

			var orderRequest = new CreateOrderRequest
			{
				OrderId = orderId,
				OrderCurrency = "INR",
				OrderAmount = orderDetails.Item3.total_price_summary_amount,
				CustomerDetails = new CustomerDetails
				{
					CustomerId = orderDetails.Item3.user_id_pk.ToString(),
					CustomerPhone = orderDetails.Item3.mobile,
					CustomerEmail = orderDetails.Item3.email,
					CustomerName = orderDetails.Item3.user_name
				},
				OrderNote = $"Payment for Order {orderId}",
				OrderMeta = new OrderMeta
				{
					ReturnUrl = Url.Action("PaymentCallback", "UserBooking", null, Request.Scheme) + "?order_id={order_id}"
					// NotifyUrl, PaymentMethods etc. can be added here if needed
				}
			};

			var responce =  await _cashfreeService.CreateOrderAsync(orderRequest);

			if (responce == null || string.IsNullOrEmpty(responce.PaymentSessionId))
			{
				await _logService.LogCustomAsync($"Cashfree returned null/empty response for OrderId: {orderId}");
				return null;
			}

			var userClaims = _tokenProvider.GetUserClaims();

			// save in db when order created successfully
			var saveOrderResult = await _unitOfWork.SPRepository.SaveCashfreeOrderDetailsAsync(responce, orderDetails.Item3, userClaims);
			if (saveOrderResult.Result <= 0)
			{
				await _logService.LogCustomAsync(saveOrderResult.message);				
				return null;
			}

			await _logService.LogCustomAsync(
					$"New Cashfree order saved. OrderId: {responce.OrderId}, " +
					$"SessionId: {responce.PaymentSessionId}, " +
					$"Expires: {responce.OrderExpiryTime}");

			return responce;
		}

		[HttpGet]
		public async Task<IActionResult> PaymentCallback(string order_id)
		{
			try
			{
				// 1. Validate order_id
				if (string.IsNullOrEmpty(order_id))
				{
					await _logService.LogCustomAsync("PaymentCallback called without order_id");
					return View("PaymentFailed", new PaymentResultViewModel
					{
						ErrorMessage = "Invalid payment reference"
					});
				}

				// 2. Fetch order status from Cashfree
				var orderStatus = await _cashfreeService.GetOrderStatusAsync(order_id);

				if (orderStatus == null)
				{
					await _logService.LogCustomAsync($"Failed to fetch order status from Cashfree. OrderId: {order_id}");
					return View("PaymentFailed", new PaymentResultViewModel
					{
						OrderId = order_id,
						ErrorMessage = "Unable to verify payment status. Please contact support."
					});
				}

				// 3. Handle ACTIVE early — no payment made yet, don't fetch payment details
				//if (orderStatus.OrderStatus == "ACTIVE")
				//{
				//	await _unitOfWork.SPRepository.UpdateOrderStatusAsync(order_id, "ACTIVE", orderStatus.PaymentSessionId);

				//	return View("PaymentPending", new PaymentResultViewModel
				//	{
				//		OrderId = order_id,
				//		Message = "Your payment is being processed. You will receive confirmation shortly."
				//	});
				//}

				//// 4. Handle EXPIRED early — session expired, no payment
				//if (orderStatus.OrderStatus == "EXPIRED")
				//{
				//	await _unitOfWork.SPRepository.UpdateOrderStatusAsync(order_id, "EXPIRED", orderStatus.PaymentSessionId);
				//	return View("PaymentFailed", new PaymentResultViewModel
				//	{
				//		OrderId = order_id,
				//		Status = "Expired",
				//		ErrorMessage = "Payment session expired. Please try booking again."
				//	});
				//}


				// 5.Update order status in DB(for PAID / failed statuses)
				var updateOrderStatusResult = await _unitOfWork.SPRepository.UpdateOrderStatusAsync(order_id, orderStatus.OrderStatus,orderStatus.PaymentSessionId);
				if (updateOrderStatusResult.Result <= 0)
				{
					await _logService.LogCustomAsync(updateOrderStatusResult.message);					
				}

				// 6. Fetch payment details (only for PAID or other terminal statuses)
				var paymentDetails = await _cashfreeService.GetPaymentDetailsAsync(order_id);

				if (paymentDetails == null || !paymentDetails.Any())
				{
					await _logService.LogCustomAsync($"No payment details found for OrderId: {order_id}");

					// If PAID but no payment details — show success anyway

					if (orderStatus.OrderStatus == "PAID")
					{
						return View("PaymentSuccess", new PaymentResultViewModel
						{
							OrderId = order_id,
							Amount = orderStatus.OrderAmount,
							Status = "Success",
							BookingId = order_id,
							TransactionId = "Pending"
						});
					}

					return View("PaymentFailed", new PaymentResultViewModel
					{
						OrderId = order_id,
						ErrorMessage = "Unable to retrieve payment details. Please contact support."
					});
				}

				var userClaims = _tokenProvider.GetUserClaims();
				bool anyPaymentSaved = false;

				// 7. Save payment details with duplicate protection
				foreach (var item in paymentDetails)
				{
					// Idempotency — skip if already saved
					var alreadyExists = await _unitOfWork.SPRepository.CashfreePaymentExistsAsync(item.CfPaymentId);
					if (alreadyExists.Item3 > 0)
					{
						await _logService.LogCustomAsync(
							$"Payment already saved, skipping. CfPaymentId: {item.CfPaymentId}");
						anyPaymentSaved = true;
						continue;
					}

					var paymentSaveToDb = await _unitOfWork.SPRepository.SaveCashfreePaymentDetailsAsync(item, userClaims);
					if (paymentSaveToDb.Result <= 0)
					{
						await _logService.LogCustomAsync(paymentSaveToDb.message);
						continue;
					}

					anyPaymentSaved = true;

					var updateBookingPaymentResult = await _unitOfWork.SPRepository.UpdateBookingPaymentDetailsAsync(item.CfPaymentId, item.OrderId);

					if (updateBookingPaymentResult.Result <= 0)
					{
						await _logService.LogCustomAsync(updateBookingPaymentResult.message);
						continue;
					}

				}

				if (!anyPaymentSaved && orderStatus.OrderStatus == "PAID")
				{
					await _logService.LogCustomAsync(
						$"PAID but no payment saved to DB. OrderId: {order_id}. " +
						$"Requires manual check.");
				}


				// 8. Show result based on final status
				switch (orderStatus.OrderStatus)
				{
					case "PAID":						

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
						//await _unitOfWork.SPRepository.UpdateOrderStatusAsync(order_id, "ACTIVE", orderStatus.PaymentSessionId);
						return View("PaymentPending", new PaymentResultViewModel
						{
							OrderId = order_id,
							Message = "Your payment is being processed. You will receive confirmation shortly."
						});
					case "EXPIRED":
						//await _unitOfWork.SPRepository.UpdateOrderStatusAsync(order_id, "EXPIRED", orderStatus.PaymentSessionId);
						return View("PaymentFailed", new PaymentResultViewModel
						{
							OrderId = order_id,
							Status = "Expired",
							ErrorMessage = "Payment session expired. Please try booking again."
						});

					default:
						// Payment failed or cancelled
						//await _logService.LogCustomAsync($"Payment failed/cancelled. OrderId: {order_id}, Status: {orderStatus.OrderStatus}");

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
