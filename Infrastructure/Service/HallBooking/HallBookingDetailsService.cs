using HallBookingBhatPara.Application.Interface.HallBooking;
using HallBookingBhatPara.Domain.Entities;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Repository;

namespace HallBookingBhatPara.Infrastructure.Service.User
{
    public class HallBookingDetailsService : Repository<hall_booking_details>, IHallBookingDetailsRepository
    {
        private readonly ApplicationDbContext _db;
        public HallBookingDetailsService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
