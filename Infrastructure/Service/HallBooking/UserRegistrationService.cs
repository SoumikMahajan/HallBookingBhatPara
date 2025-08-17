using HallBookingBhatPara.Application.Interface.HallBooking;
using HallBookingBhatPara.Domain.Entities;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Repository;

namespace HallBookingBhatPara.Infrastructure.Service.User
{
    public class UserRegistrationService : Repository<public_user_registration>, IUserRegistrationRepository
    {
        private readonly ApplicationDbContext _db;
        public UserRegistrationService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
