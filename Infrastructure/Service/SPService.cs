using Dapper;
using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.DTO.Admin;
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
                    parameters.Add("@UserName", model.FirstName, DbType.String);
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
        #endregion
    }
}
