using Dapper;
using HallBookingBhatPara.Domain.DTO;
using HallBookingBhatPara.Domain.DTO.Admin;
using HallBookingBhatPara.Domain.DTO.CashFreePayment;
using HallBookingBhatPara.Domain.DTO.HallBooking;
using HallBookingBhatPara.Domain.DTO.User;
using Microsoft.Data.SqlClient;
using System.Data;

namespace HallBookingBhatPara.Application.Interface
{
    public interface ISPRepository
    {
        #region :: User
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO model);
        Task<long> RegistrationAsync(UserRegistrationDto model);
        #endregion

        #region :: HallType
        Task<long> AddHallCategoryAsync(string categoryName);
        Task<long> AddHallSubCategoryAsync(InsertSubCategoryDTO model);
        Task<List<SubCategorieDTO>> GetALlSubcategorisAsync();
        Task<long> AddHallAvailableAsync(InsertHallAvailableDTO model);
        Task<List<HallAvailableDTO>> GetAllHallAvailableAsync();
        Task<GetHallAvailableDTO> GetHallAvailableDetailsByIdAsync(long hallAvailabilityId);
        Task<List<DropDownListDTO>> GetFloorListBySubCatIdAsync(long subCategoryId);
        Task<int> CheckDatesOfHallAvailAsync(InsertHallAvailableDTO model);
        Task<long> UpdateHallAvailableAsync(UpdateHallAvailableDTO model);
        Task<int> CheckDatesOfHallAvailInUpdateAsync(UpdateHallAvailableDTO model);
        Task<string> UpdateProfileImagePathAsync(long userId, string imagePath);

        Task<List<HallSearchDTO>> HallAvailableSearchResultAsync(long catType, string startDate, string endDate);
        Task<HallBookingDTO> GetHallDetailsAfterSearchAsync(long hallAvlId);
        Task<long> BookUserConfirmedHallAsync(InsertUserConfirmhallDTO model, int dateCount, double TotalPriceSummery, string bookingId);

        Task<long> BookUserConfirmedHallForCounterAdminAsync(InsertUserConfirmhallForCounterAdminDTO model, int PercentageOfIntialPaymentAmount, double RemainingAmount, int dateCount, double TotalPriceSummery, string bookingId);
		Task<PaymentSummeryDTO> GetPaymentSummeryDetailsAsync(long hallAvailId, long PaymentId, long PercentageOfIntialPaymentAmount);
        Task<PaymentSummeryDTO> GetPaymentSummeryDetailsForMultipleDateAsync(long hallAvailId, long PaymentId, long PercentageOfIntialPaymentAmount, int dateCount);
        Task<int> IsEventDateAlreadyBookedAsync(long hallAvailId, string eventDate);
        Task<List<BookedListDTO>> PublicUserHallBookedDetailsAsync(long loggedInRoleId,long loggedInStackId);

		Task<List<UserListDTO>> GetAllUserListForHallBooking();
		Task<BookedDetailsByIdDTO> BookingDetailsByIdAsync(long HallId);

		#endregion

		#region
		Task<(int Result,string message, OrderDetailsForPaymentDTO)> GetBookingDetailsByReferenceIdAsync(string orderid);
        Task<(int Result, string message,long)> SaveCashfreeOrderDetailsAsync(CreateOrderResponse model, OrderDetailsForPaymentDTO model2,UserClaims userClaims);
        Task<(int Result, string message)> UpdateOrderStatusAsync(string OrderId, string OrderStatus, string PaymentSessionId);
        Task<(int Result, string message,long)> SaveCashfreePaymentDetailsAsync(PaymentDetails model, UserClaims userClaims);
        Task<(int Result, string message)> UpdateBookingPaymentDetailsAsync(string CfPaymentId,string OrderId);
		Task<(int Result, string message,ExistingCashfreeOrder)> GetActiveCashfreeOrderAsync(string orderId);
		Task<(int Result, string message, int)> CashfreePaymentExistsAsync(string CfPaymentId);
		#endregion

		#region :: Admin

		Task<List<HallBookingDetailsDTO>> UserHallBookedDetailsAsync(long StakeId,long StakeDetailsId);
		Task<List<UserListForAdminDTO>> GetUserListOnAdminAsync(long RoleId);
		Task<UserDetailsForAdminDTO> GetUserListOnAdminAsync(long UserId, int roleId);
		Task<string> UpdateUserOnAdminAsync(EditUserDto model);
        Task<List<DropDownListDTO>> UserRoleAsync();
        Task<string> ApproveHallAsync(long BookingId, string BookingReferenceId,long UpdateBy,long loggedInRoleId,string EntryIP);
		Task<string> RejectHallAsync(long BookingId, string BookingReferenceId, long UpdateBy, long loggedInRoleId, string EntryIP);
        Task<long> AdminUserAddAsync(AddUserDto model);
		Task<bool> IsEmailExistsAsync(string email);
        Task<bool> IsMobileExistsAsync(string mobile);

		#endregion
	}
}
