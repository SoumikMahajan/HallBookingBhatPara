using HallBookingBhatPara.Application.Interface.User;
using HallBookingBhatPara.Domain.Entities;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Repository;

namespace HallBookingBhatPara.Infrastructure.Service.User
{
    public class CategoryMasterService : Repository<hall_category_master>, ICategoryMasterRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryMasterService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
