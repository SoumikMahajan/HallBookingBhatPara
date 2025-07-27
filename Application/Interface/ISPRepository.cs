using HallBookingBhatPara.Domain.DTO.User;

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
        #endregion
    }
}
