using HallBookingBhatPara.Domain.DTO.Admin;
using HallBookingBhatPara.Domain.Entities;

namespace HallBookingBhatPara.Application.Interface.User
{
    public interface IHallAvailMasterRepository : IRepository<hall_availability_details>
    {
        Task<bool> UpdateAsync(UpdateHallAvailableDTO model);
    }
}
