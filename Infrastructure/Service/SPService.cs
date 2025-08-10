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

        public async Task<List<HallSearchDTO>> HallAvailableSearchResultAsync(long hallType, string startDate, string endDate)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@HallId", hallType, DbType.Int64);
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

        #endregion
    }
}
