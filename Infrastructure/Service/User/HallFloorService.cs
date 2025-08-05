using HallBookingBhatPara.Application.Interface.User;
using HallBookingBhatPara.Domain.Entities;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Repository;

namespace HallBookingBhatPara.Infrastructure.Service.User
{
    public class HallFloorService : Repository<hall_floor_map_details>, IHallFloorMasterRepository
    {
        private readonly ApplicationDbContext _db;
        public HallFloorService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

    }
}
