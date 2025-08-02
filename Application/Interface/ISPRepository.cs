using HallBookingBhatPara.Domain.DTO.Admin;
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
        Task<long> AddHallSubCategoryAsync(InsertSubCategoryDTO model);
        Task<List<SubCategorieDTO>> GetALlSubcategorisAsync();
        Task<long> AddHallAvailableAsync(InsertHallAvailableDTO model);
        Task<List<HallAvailableDTO>> GetAllHallAvailableAsync();
        #endregion
    }
}
