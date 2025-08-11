using HallBookingBhatPara.Application.Interface.HallBooking;
using HallBookingBhatPara.Domain.Entities;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Repository;

namespace HallBookingBhatPara.Infrastructure.Service.User
{
    public class HallEventMasterService : Repository<hall_event_type_master>, IHallEventMasterRepository
    {
        private readonly ApplicationDbContext _db;
        public HallEventMasterService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
