using Dapper;
using HallBookingBhatPara.Application.Interface;
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
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Name", model.FirstName, DbType.String);
                parameters.Add("@Email", model.Email, DbType.String);
                parameters.Add("@HashedPassword", PasswordHasher.ComputeSha256Hash(model.Password), DbType.String);
                parameters.Add("@OperationId", 2, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<long>(
                    "Bhatpara_HallBooking_Users",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }
        #endregion


        #region :: HallType
        public async Task<long> AddHallCategoryAsync(string categoryName)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                var parameters = new DynamicParameters();
                parameters.Add("@categoryName", categoryName, DbType.String);
                parameters.Add("@OperationId", 1, DbType.Int32);
                var result = await connection.QueryFirstOrDefaultAsync<long>(
                    "adminHallCategorySp",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return result;
            }
        }
        #endregion
    }
}
