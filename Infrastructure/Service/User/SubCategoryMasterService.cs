using HallBookingBhatPara.Application.Interface.User;
using HallBookingBhatPara.Domain.DTO.Admin;
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
        public async Task<bool> UpdateAsync(UpdateSubCategoryDTO model)
        {
            var hall = await _db.hall_master.FindAsync(model.Subcategoryid);
            if (hall == null)
                return false;

            hall.category_id_fk = model.CategoryId;
            hall.hall_name = model.SubCategoryName;

            if (model.HasNewImage == true && model.ImageData != null)
            {
                hall.hall_image = model.ImageData;
            }

            _db.hall_master.Update(hall);
            return await _db.SaveChangesAsync() > 0; // Returns true if changes were saved successfully
        }
        //public async Task<bool> UpdateWithOutImage(UpdateSubCategoryDTO model)
        //{
        //    var hall = await _db.hall_master.FindAsync(model.Subcategoryid);
        //    if (hall == null)
        //    {
        //        return false; // Hall not found
        //    }
        //    hall.category_id_fk = model.CategoryId;
        //    hall.hall_name = model.SubCategoryName;

        //    _db.hall_master.Update(hall);
        //    return await _db.SaveChangesAsync() > 0; // Returns true if changes were saved successfully
        //}
    }
}
