using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Infrastructure.Data;

namespace HallBookingBhatPara.Infrastructure.Repository
{
    public class SPService : ISPRepository
    {
        private readonly ApplicationDbContext _db;
        public SPService(ApplicationDbContext db)
        {
            _db = db;
        }
    }
}
