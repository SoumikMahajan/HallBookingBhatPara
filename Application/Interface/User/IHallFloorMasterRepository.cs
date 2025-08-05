using HallBookingBhatPara.Domain.Entities;

namespace HallBookingBhatPara.Application.Interface.User
{
    public interface IHallFloorMasterRepository : IRepository<hall_floor_map_details>
    {
        //Task<bool> UpdateAsync(UpdateHallAvailableDTO model);
    }
}
