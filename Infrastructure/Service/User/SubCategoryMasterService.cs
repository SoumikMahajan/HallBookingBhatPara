using HallBookingBhatPara.Application.Interface.User;
using HallBookingBhatPara.Domain.Entities;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Repository;

namespace HallBookingBhatPara.Infrastructure.Service.User
{
    public class SubCategoryMasterService : Repository<hall_master>, ISubCategoryMasterRepository
    {
        private readonly ApplicationDbContext _db;
        public SubCategoryMasterService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
