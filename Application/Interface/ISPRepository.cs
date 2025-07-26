using HallBookingBhatPara.Domain.DTO.User;

namespace HallBookingBhatPara.Application.Interface
{
    public interface ISPRepository
    {
        #region :: User
        Task<LoginResponseDTO> LoginAsync(LoginRequestDTO model);
        #endregion
    }
}
