using Dapper;
using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.DTO;
using HallBookingBhatPara.Domain.DTO.Admin;
using HallBookingBhatPara.Domain.DTO.HallBooking;
using HallBookingBhatPara.Domain.DTO.User;
using HallBookingBhatPara.Domain.Utility;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace HallBookingBhatPara.Infrastructure.Repository
{
    public class SPService : ISPRepository
    {
        private readonly string _connectionString;
        private readonly LogService _logService;
        public SPService(IConfiguration configuration, LogService logService)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
            _logService = logService;
        }


        #region :: User
        public async Task<LoginResponseDTO> LoginAsync(LoginRequestDTO model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Email", model.Email, DbType.String);
                parameters.Add("@HashedPassword", PasswordHasher.ComputeSha256Hash(model.Password), DbType.String);
                parameters.Add("@OperationId", 1, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<LoginResponseDTO>(
                    "Bhatpara_HallBooking_Users",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        public async Task<long> RegistrationAsync(UserRegistrationDto model)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@UserName", $"{model.FirstName} {model.LastName}", DbType.String);
                    parameters.Add("@Mobile", model.Phone, DbType.String);
                    parameters.Add("@Email", model.Email, DbType.String);
                    parameters.Add("@EntryIp", model.EntryIP, DbType.String);
                    parameters.Add("@Gender", model.Gender, DbType.Int64);
                    parameters.Add("@DOB", model.DOB, DbType.Date);
                    parameters.Add("@Address", model.Address, DbType.String);
                    parameters.Add("@City", model.City, DbType.String);
                    parameters.Add("@Pin", model.Pincode, DbType.String);
                    parameters.Add("@LoginPassword", model.Password, DbType.String);
                    parameters.Add("@BasePassword", model.BasePassword, DbType.String);
                    parameters.Add("@ProfileImg", model.ImageData, DbType.Binary);
                    parameters.Add("@OperationId", 2, DbType.Int32);
                    var result = await connection.QueryFirstOrDefaultAsync<long>(
                        "Bhatpara_HallBooking_Users",
                        parameters,
                        commandType: CommandType.StoredProcedure
                    );
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Registration failed: " + ex.Message);
                throw;
            }

        }
        #endregion


        #region :: Admin
        public async Task<long> AddHallCategoryAsync(string categoryName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@categoryName", categoryName, DbType.String);
                parameters.Add("@OperationId", 3, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<long>(
                    "adminHallMasterSp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }
        public async Task<long> AddHallSubCategoryAsync(InsertSubCategoryDTO model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CategoryId", model.CategoryId, DbType.Int64);
                parameters.Add("@HallName", model.SubCategoryName, DbType.String);
                parameters.Add("@HallImg", model.ImageData, DbType.Binary);
                parameters.Add("@CreatedBy", model.CreatedBy, DbType.Int64);
                parameters.Add("@OperationId", 1, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<long>(
                    "adminHallMasterSp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }
        public async Task<List<SubCategorieDTO>> GetALlSubcategorisAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@OperationId", 2, DbType.Int32);
                var result = await connection.QueryAsync<SubCategorieDTO>(
                    "adminHallMasterSp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }
        public async Task<long> AddHallAvailableAsync(InsertHallAvailableDTO model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HallCategoryId", model.CategoryId, DbType.Int64);
                parameters.Add("@HallId", model.SubcategoryId, DbType.Int64);
                parameters.Add("@AvalFromDate", model.AvailableFrom, DbType.Date);
                parameters.Add("@AvalToDate", model.AvailableTo, DbType.Date);
                parameters.Add("@Rate", model.ProposedRate, DbType.Decimal);
                parameters.Add("@SecurityMoney", model.SecurityMoney, DbType.Decimal);
                parameters.Add("@FloorId", model.FloorId, DbType.Int64);
                parameters.Add("@PaymentTypeId", model.PaymentTypeId, DbType.Int64);
                parameters.Add("@GlobalUserId", model.userClaims.StackHolderId, DbType.Int64);
                parameters.Add("@RoleId", model.userClaims.RolesId, DbType.Int64);
                parameters.Add("@OperationId", 1, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<long>(
                    "adminHallAvailabilitySp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }
        public async Task<List<HallAvailableDTO>> GetAllHallAvailableAsync()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@OperationId", 2, DbType.Int32);
                var result = await connection.QueryAsync<HallAvailableDTO>(
                    "adminHallAvailabilitySp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }
        public async Task<GetHallAvailableDTO> GetHallAvailableDetailsByIdAsync(long hallAvailabilityId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HallAvilableId", hallAvailabilityId, DbType.Int64);
                parameters.Add("@OperationId", 3, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<GetHallAvailableDTO>(
                    "adminHallAvailabilitySp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        public async Task<List<DropDownListDTO>> GetFloorListBySubCatIdAsync(long subCategoryId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HallCategoryId", subCategoryId, DbType.Int64);
                parameters.Add("@OperationId", 4, DbType.Int32);
                var result = await connection.QueryAsync<DropDownListDTO>(
                    "adminHallAvailabilitySp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }
        public async Task<int> CheckDatesOfHallAvailAsync(InsertHallAvailableDTO model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HallCategoryId", model.CategoryId, DbType.Int64);
                parameters.Add("@HallId", model.SubcategoryId, DbType.Int64);
                parameters.Add("@AvalFromDate", model.AvailableFrom, DbType.Date);
                parameters.Add("@AvalToDate", model.AvailableTo, DbType.Date);
                parameters.Add("@FloorId", model.FloorId, DbType.Int64);
                parameters.Add("@OperationId", 5, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "adminHallAvailabilitySp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        public async Task<long> UpdateHallAvailableAsync(UpdateHallAvailableDTO model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HallAvailabilityIdPk", model.HallAvailId, DbType.Int64);
                parameters.Add("@HallCategoryId", model.CategoryId, DbType.Int64);
                parameters.Add("@HallId", model.SubcategoryId, DbType.Int64);
                parameters.Add("@AvalFromDate", model.AvailableFrom, DbType.Date);
                parameters.Add("@AvalToDate", model.AvailableTo, DbType.Date);
                parameters.Add("@Rate", model.ProposedRate, DbType.Decimal);
                parameters.Add("@SecurityMoney", model.SecurityMoney, DbType.Decimal);
                parameters.Add("@GlobalUserId", model.userClaims.StackHolderId, DbType.Int64);
                parameters.Add("@RoleId", model.userClaims.RolesId, DbType.Int64);
                parameters.Add("@FloorId", model.FloorId, DbType.Int64);
                parameters.Add("@PaymentTypeId", model.PaymentTypeId, DbType.Int64);
                parameters.Add("@OperationId", 6, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<long>(
                    "adminHallAvailabilitySp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        public async Task<int> CheckDatesOfHallAvailInUpdateAsync(UpdateHallAvailableDTO model)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HallAvailabilityIdPk", model.HallAvailId, DbType.Int64);
                parameters.Add("@HallCategoryId", model.CategoryId, DbType.Int64);
                parameters.Add("@HallId", model.SubcategoryId, DbType.Int64);
                parameters.Add("@AvalFromDate", model.AvailableFrom, DbType.Date);
                parameters.Add("@AvalToDate", model.AvailableTo, DbType.Date);
                parameters.Add("@FloorId", model.FloorId, DbType.Int64);
                parameters.Add("@OperationId", 7, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "adminHallAvailabilitySp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        public async Task<string> UpdateProfileImagePathAsync(long userId, string imagePath)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@LoginId", userId, DbType.Int64);
                parameters.Add("@ProfilePicPath", imagePath, DbType.String);
                parameters.Add("@OperationId", 3, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<string>(
                    "Bhatpara_HallBooking_Users",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        public async Task<List<HallSearchDTO>> HallAvailableSearchResultAsync(long catType, string startDate, string endDate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CatId", catType, DbType.Int64);
                parameters.Add("@AvalFromDate", startDate, DbType.Date);
                parameters.Add("@AvalToDate", endDate, DbType.Date);
                parameters.Add("@OperationId", 4, DbType.Int32);
                var result = await connection.QueryAsync<HallSearchDTO>(
                    "Bhatpara_HallBooking_Users",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }

        public async Task<HallBookingDTO> GetHallDetailsAfterSearchAsync(long hallAvlId)
        {
            //using (var connection = new SqlConnection(_connectionString))
            //{
            //    var parameters = new DynamicParameters();
            //    parameters.Add("@HallAvlId", hallAvlId, DbType.Int64);
            //    parameters.Add("@OperationId", 5, DbType.Int32);
            //    var result = await connection.QueryFirstOrDefaultAsync<HallBookingDTO>(
            //        "Bhatpara_HallBooking_Users",
            //        parameters,
            //        commandType: CommandType.StoredProcedure
            //    );
            //    return result;
            //}

            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HallAvlId", hallAvlId, DbType.Int64);
                parameters.Add("@OperationId", 5, DbType.Int32);

                using (var multi = await connection.QueryMultipleAsync(
                    "Bhatpara_HallBooking_Users",
                    parameters,
                    commandType: CommandType.StoredProcedure))
                {
                    // First result set — single booking
                    var booking = await multi.ReadFirstOrDefaultAsync<HallBookingDTO>();

                    if (booking == null)
                        return null;

                    // Second result set — dates for that booking
                    booking.hallAvailableDateDTOs = (await multi.ReadAsync<HallAvailableDateDTO>()).ToList();

                    return booking;
                }
            }
        }

        public async Task<long> BookUserConfirmedHallAsync(InsertUserConfirmhallDTO model, int dateCount, double TotalPriceSummery, string bookingId)
        {
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				parameters.Add("@CatId", model.catId, DbType.Int64);
				parameters.Add("@HallId", model.hallId, DbType.Int64);
				parameters.Add("@HallAvlId", model.hallAvailId, DbType.Int64);
				parameters.Add("@Rate", model.PaymentSummeryDTO.rate, DbType.Decimal);
				parameters.Add("@SecurityMoney", model.PaymentSummeryDTO.security_money, DbType.Decimal);
				parameters.Add("@BookedUserName", model.fullName, DbType.String);
				parameters.Add("@BookedUserMobile", model.phone, DbType.String);
				parameters.Add("@BookedUserAlterMobile", model.alternatePhone, DbType.String);
				parameters.Add("@BookedUserEmail", model.email, DbType.String);
				parameters.Add("@BookedUserAddress", model.address, DbType.String);
				parameters.Add("@EventTypeId", model.eventType, DbType.Int64);
				parameters.Add("@EntryIp", model.EntryIP, DbType.String);
				parameters.Add("@StakeId", model.userClaims.RolesId, DbType.Int64);
				parameters.Add("@StakeDetailsId", model.userClaims.StackHolderId, DbType.Int64);
				parameters.Add("@PaymentTypeId", model.OnloadPaymentTypeId, DbType.Int64);
				parameters.Add("@BookedDayCount", dateCount, DbType.Int32);

				//-----------------------------------------------------------------------------------//
				//-----------------------------------------------------------------------------------//

				//parameters.Add("@PaymentPercentage", PercentageOfIntialPaymentAmount, DbType.Int64);
				//parameters.Add("@PaymentAmount", model.PaymentSummeryDTO.payable_amount, DbType.Decimal);
				//parameters.Add("@RemainingPaymentAmount", RemainingAmount, DbType.Decimal);
				parameters.Add("@TotalPriceSummaryAmount", TotalPriceSummery, DbType.Decimal);

				//-----------------------------------------------------------------------------------//
				//-----------------------------------------------------------------------------------//

				parameters.Add("@BookingDate", model.eventDate, DbType.String);
				parameters.Add("@BookingRefId", bookingId, DbType.String);



				parameters.Add("@OperationId", 5, DbType.Int32);
				var result = await connection.QueryFirstOrDefaultAsync<long>(
					"UserHallBookingSp",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				return result;
			}


			//using (var connection = new SqlConnection(_connectionString))
			//{
			//    var parameters = new DynamicParameters();
			//    parameters.Add("@CatId", model.catId, DbType.Int64);
			//    parameters.Add("@HallId", model.hallId, DbType.Int64);
			//    parameters.Add("@HallAvlId", model.hallAvailId, DbType.Int64);
			//    parameters.Add("@Rate", model.PaymentSummeryDTO.rate, DbType.Decimal);
			//    parameters.Add("@SecurityMoney", model.PaymentSummeryDTO.security_money, DbType.Decimal);
			//    parameters.Add("@BookedUserName", model.fullName, DbType.String);
			//    parameters.Add("@BookedUserMobile", model.phone, DbType.String);
			//    parameters.Add("@BookedUserAlterMobile", model.alternatePhone, DbType.String);
			//    parameters.Add("@BookedUserEmail", model.email, DbType.String);
			//    parameters.Add("@BookedUserAddress", model.address, DbType.String);
			//    parameters.Add("@EventTypeId", model.eventType, DbType.Int64);
			//    parameters.Add("@EntryIp", model.EntryIP, DbType.String);
			//    parameters.Add("@StakeId", model.userClaims.RolesId, DbType.Int64);
			//    parameters.Add("@StakeDetailsId", model.userClaims.StackHolderId, DbType.Int64);
			//    parameters.Add("@PaymentTypeId", model.OnloadPaymentTypeId, DbType.Int64);
			//    parameters.Add("@BookedDayCount", dateCount, DbType.Int32);

			//    //-----------------------------------------------------------------------------------//
			//    //-----------------------------------------------------------------------------------//

			//    parameters.Add("@PaymentPercentage", PercentageOfIntialPaymentAmount, DbType.Int64);
			//    parameters.Add("@PaymentAmount", model.PaymentSummeryDTO.payable_amount, DbType.Decimal);
			//    parameters.Add("@RemainingPaymentAmount", RemainingAmount, DbType.Decimal);
			//    parameters.Add("@TotalPriceSummaryAmount", TotalPriceSummery, DbType.Decimal);

			//    //-----------------------------------------------------------------------------------//
			//    //-----------------------------------------------------------------------------------//

			//    parameters.Add("@BookingDate", model.eventDate, DbType.String);
			//    parameters.Add("@BookingRefId", bookingId, DbType.String);



			//    parameters.Add("@OperationId", 1, DbType.Int32);
			//    var result = await connection.QueryFirstOrDefaultAsync<long>(
			//        "UserHallBookingSp",
			//        parameters,
			//        commandType: CommandType.StoredProcedure
			//    );
			//    return result;
			//}
		}

        public async Task<long> BookUserConfirmedHallForCounterAdminAsync (InsertUserConfirmhallForCounterAdminDTO model, int PercentageOfIntialPaymentAmount, double RemainingAmount, int dateCount, double TotalPriceSummery, string bookingId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CatId", model.catId, DbType.Int64);
                parameters.Add("@HallId", model.hallId, DbType.Int64);
                parameters.Add("@HallAvlId", model.hallAvailId, DbType.Int64);
                parameters.Add("@Rate", model.PaymentSummeryDTO.rate, DbType.Decimal);
                //parameters.Add("@SecurityMoney", model.PaymentSummeryDTO.security_money, DbType.Decimal);
				parameters.Add("@BookingRefId", bookingId, DbType.String);
				parameters.Add("@BookedUserName", model.fullName, DbType.String);
                parameters.Add("@BookedUserMobile", model.phone, DbType.String);
                parameters.Add("@BookedUserEmail", model.email, DbType.String);
                parameters.Add("@EventTypeId", model.eventType, DbType.Int64);
                parameters.Add("@EntryIp", model.EntryIP, DbType.String);
                parameters.Add("@StakeId", model.userClaims.RolesId, DbType.Int64);
                parameters.Add("@StakeDetailsId", model.userClaims.StackHolderId, DbType.Int64);
                parameters.Add("@PaymentTypeId", model.OnloadPaymentTypeId, DbType.Int64);
				parameters.Add("@BookedDayCount", dateCount, DbType.Int32);
				parameters.Add("@TotalPriceSummaryAmount", TotalPriceSummery, DbType.Decimal);
				parameters.Add("@PaymentTransID", model.MrNumber, DbType.String);

				//-----------------------------------------------------------------------------------//
				//-----------------------------------------------------------------------------------//
				//parameters.Add("@PaymentPercentage", PercentageOfIntialPaymentAmount, DbType.Int64);
                parameters.Add("@PaymentAmount", model.PaymentSummeryDTO.payable_amount, DbType.Decimal);
                //parameters.Add("@RemainingPaymentAmount", RemainingAmount, DbType.Decimal);

                //-----------------------------------------------------------------------------------//
                //-----------------------------------------------------------------------------------//
                parameters.Add("@BookingDate", model.eventDate, DbType.String);
                
                parameters.Add("@OperationId", 3, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<long>(
                    "UserHallBookingSp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
			}
		}


		public async Task<PaymentSummeryDTO> GetPaymentSummeryDetailsAsync(long hallAvailId, long PaymentId, long PercentageOfIntialPaymentAmount)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HallAvlId", hallAvailId, DbType.Int64);
                parameters.Add("@PaymentTypeId", PaymentId, DbType.Int64);
                parameters.Add("@PaymentPercentage", PercentageOfIntialPaymentAmount, DbType.Int64);
                parameters.Add("@OperationId", 7, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<PaymentSummeryDTO>(
                    "Bhatpara_HallBooking_Users",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }
        public async Task<PaymentSummeryDTO> GetPaymentSummeryDetailsForMultipleDateAsync(long hallAvailId, long PaymentId, long PercentageOfIntialPaymentAmount, int dateCount)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HallAvlId", hallAvailId, DbType.Int64);
                parameters.Add("@PaymentTypeId", PaymentId, DbType.Int64);
                parameters.Add("@PaymentPercentage", PercentageOfIntialPaymentAmount, DbType.Int64);
                parameters.Add("@DateCount", dateCount, DbType.Int32);
                parameters.Add("@OperationId", 8, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<PaymentSummeryDTO>(
                    "Bhatpara_HallBooking_Users",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        public async Task<int> IsEventDateAlreadyBookedAsync(long hallAvailId, string eventDate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HallAvlId", hallAvailId, DbType.Int64);
                parameters.Add("@BookingDate", eventDate, DbType.String);
                parameters.Add("@OperationId", 6, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<int>(
                    "Bhatpara_HallBooking_Users",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }

        public async Task<List<BookedListDTO>> PublicUserHallBookedDetailsAsync(long loggedInRoleId, long loggedInStackId)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
				parameters.Add("@StakeId", loggedInRoleId, DbType.Int64);
				parameters.Add("@StakeDetailsId", loggedInStackId, DbType.Int64);
                parameters.Add("@OperationId", 2, DbType.Int32);
                var result = await connection.QueryAsync<BookedListDTO>(
                    "UserHallBookingSp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result.ToList();
            }
        }

		public async Task<List<UserListDTO>> GetAllUserListForHallBooking()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();				
				parameters.Add("@OperationId", 10, DbType.Int32);
				var result = await connection.QueryAsync<UserListDTO>(
					"Bhatpara_HallBooking_Users",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				return result.ToList();
			}
		}

		public async Task<BookedDetailsByIdDTO> BookingDetailsByIdAsync(long HallId)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				parameters.Add("@HallId", HallId, DbType.Int64);				
				parameters.Add("@OperationId", 8, DbType.Int32);

				var result = await connection.QueryFirstOrDefaultAsync<BookedDetailsByIdDTO>(
					"UserHallBookingSp",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				return result;
			}
		}

		public async Task<List<HallBookingDetailsDTO>> UserHallBookedDetailsAsync(long StakeId, long StakeDetailsId)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				parameters.Add("@StakeId", StakeId, DbType.Int64);
				parameters.Add("@StakeDetailsId", StakeDetailsId, DbType.Int64);
				parameters.Add("@OperationId", 4, DbType.Int32);

				var result = await connection.QueryAsync<HallBookingDetailsDTO>(
					"UserHallBookingSp",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				return result.ToList();
			}
		}

        public async Task<List<UserListForAdminDTO>> GetUserListOnAdminAsync(long RoleId)
        {
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				parameters.Add("@StakeId", RoleId, DbType.Int64);				
				parameters.Add("@OperationId", 12, DbType.Int32);

				var result = await connection.QueryAsync<UserListForAdminDTO>(
					"Bhatpara_HallBooking_Users",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				return result.ToList();
			}
		}

		public async Task<UserDetailsForAdminDTO> GetUserListOnAdminAsync(long UserId, int roleId)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				parameters.Add("@UserId", UserId, DbType.Int64);
				parameters.Add("@StakeId", roleId, DbType.Int32);
				parameters.Add("@OperationId", 13, DbType.Int32);

				var result = await connection.QueryFirstOrDefaultAsync<UserDetailsForAdminDTO>(
					"Bhatpara_HallBooking_Users",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				return result;
			}
		}

		public async Task<string> UpdateUserOnAdminAsync(EditUserDto model)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				parameters.Add("@StakeId", model.RoleId, DbType.Int32);
				parameters.Add("@UserName", $"{model.FirstName} {model.LastName}" , DbType.String);				
				parameters.Add("@Gender", model.Gender, DbType.Int64);
				parameters.Add("@DOB", model.DOB, DbType.Date);
				parameters.Add("@Address", model.Address, DbType.String);
				parameters.Add("@City", model.City, DbType.String);
				parameters.Add("@Pin", model.Pincode, DbType.String);
				parameters.Add("@UserId", model.UserId, DbType.Int64);
				parameters.Add("@loginId", model.UpdateBy, DbType.Int64);


				parameters.Add("@OperationId", 14, DbType.Int32);
				var result = await connection.QueryFirstOrDefaultAsync<string>(
					"Bhatpara_HallBooking_Users",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				return result;
			}
		}

		public async Task<List<DropDownListDTO>> UserRoleAsync()
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();
				
				parameters.Add("@OperationId", 15, DbType.Int32);

				var result = await connection.QueryAsync<DropDownListDTO>(
					"Bhatpara_HallBooking_Users",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				return result.ToList();
			}
		}

		public async Task<string> ApproveHallAsync(long BookingId, string BookingReferenceId, long UpdateBy, long loggedInRoleId,string EntryIP)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				parameters.Add("@StakeId", loggedInRoleId, DbType.Int64);
				parameters.Add("@LoginId", UpdateBy, DbType.Int64);
				parameters.Add("@booking_id_pk", BookingId, DbType.Int64);
				parameters.Add("@BookingRefId", BookingReferenceId, DbType.String);
				parameters.Add("@EntryIp", EntryIP, DbType.String);

				parameters.Add("@OperationId", 6, DbType.Int32);
				var result = await connection.QueryFirstOrDefaultAsync<string>(
					"UserHallBookingSp",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				return result;
			}
		}
		public async Task<string> RejectHallAsync(long BookingId, string BookingReferenceId, long UpdateBy, long loggedInRoleId, string EntryIP)
		{
			using (var connection = new SqlConnection(_connectionString))
			{
				var parameters = new DynamicParameters();

				parameters.Add("@StakeId", loggedInRoleId, DbType.Int64);
				parameters.Add("@LoginId", UpdateBy, DbType.Int64);
				parameters.Add("@booking_id_pk", BookingId, DbType.Int64);
				parameters.Add("@BookingRefId", BookingReferenceId, DbType.String);
				parameters.Add("@EntryIp", EntryIP, DbType.String);

				parameters.Add("@OperationId", 7, DbType.Int32);
				var result = await connection.QueryFirstOrDefaultAsync<string>(
					"UserHallBookingSp",
					parameters,
					commandType: CommandType.StoredProcedure
				);
				return result;
			}
		}

		public async Task<long> AdminUserAddAsync(AddUserDto model)
		{
			try
			{
				using (var connection = new SqlConnection(_connectionString))
				{
					var parameters = new DynamicParameters();
					parameters.Add("@UserName", $"{model.FirstName} {model.LastName}", DbType.String);
					parameters.Add("@Mobile", model.Phone, DbType.String);
					parameters.Add("@Email", model.Email.ToLower(), DbType.String);
					parameters.Add("@EntryIp", model.EntryIP, DbType.String);
					parameters.Add("@Gender", model.Gender, DbType.Int64);
					parameters.Add("@DdlRoleId", model.Role, DbType.Int32);
					parameters.Add("@DOB", model.DOB, DbType.Date);
					parameters.Add("@Address", model.Address, DbType.String);
					parameters.Add("@City", model.City, DbType.String);
					parameters.Add("@Pin", model.Pincode, DbType.String);
					parameters.Add("@LoginPassword", model.Password, DbType.String);
					parameters.Add("@BasePassword", model.BasePassword, DbType.String);
					parameters.Add("@LoginId", model.CreatedBy, DbType.Int64);
					parameters.Add("@StakeId", model.StackRoleId, DbType.Int64);
					parameters.Add("@OperationId", 9, DbType.Int32);
					var result = await connection.QueryFirstOrDefaultAsync<long>(
						"Bhatpara_HallBooking_Users",
						parameters,
						commandType: CommandType.StoredProcedure
					);
					return result;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Registration failed: " + ex.Message);
				throw;
			}

		}

		public async Task<bool> IsEmailExistsAsync(string email)
		{
			using var connection = new SqlConnection(_connectionString);

			var parameters = new DynamicParameters();
			parameters.Add("@Email", email.ToLower(), DbType.String);
			parameters.Add("@OperationId", 16, DbType.Int32);


			return await connection.ExecuteScalarAsync<bool>(
		        "dbo.Bhatpara_HallBooking_Users",
		        parameters,
		        commandType: CommandType.StoredProcedure
	        );
		}
		public async Task<bool> IsMobileExistsAsync(string mobile)
		{
			using var connection = new SqlConnection(_connectionString);

			var parameters = new DynamicParameters();
			parameters.Add("@Mobile", mobile, DbType.String);
			parameters.Add("@OperationId", 17, DbType.Int32);


			return await connection.ExecuteScalarAsync<bool>(
				"dbo.Bhatpara_HallBooking_Users",
				parameters,
				commandType: CommandType.StoredProcedure
			);
		}

		#endregion


	}
}
