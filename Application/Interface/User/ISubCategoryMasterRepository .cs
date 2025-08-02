using HallBookingBhatPara.Domain.DTO.Admin;
using HallBookingBhatPara.Domain.Entities;

namespace HallBookingBhatPara.Application.Interface.User
{
    public interface ISubCategoryMasterRepository : IRepository<hall_master>
    {
        Task<bool> UpdateAsync(UpdateSubCategoryDTO model);
        //Task<bool> UpdateWithOutImage(UpdateSubCategoryDTO model);
    }
}
