using HallBookingBhatPara.Application.Interface.User;
using HallBookingBhatPara.Domain.Entities;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace HallBookingBhatPara.Infrastructure.Service.User
{
    public class CategoryMasterService : Repository<hall_category_master>, ICategoryMasterRepository
    {
        private readonly ApplicationDbContext _db;
        public CategoryMasterService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<bool> UpdateAsync(long categoryId, string categoryName)
        {
            var category = await _db.hall_category_master.FirstOrDefaultAsync(x => x.category_id_pk == categoryId);
            if (category == null)
                return false;

            category.category_name = categoryName;
            _db.hall_category_master.Update(category);

            var result = await _db.SaveChangesAsync();
            return result > 0;
        }
    }
}
