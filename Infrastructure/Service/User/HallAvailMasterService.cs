using HallBookingBhatPara.Application.Interface.User;
using HallBookingBhatPara.Domain.DTO.Admin;
using HallBookingBhatPara.Domain.Entities;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Repository;

namespace HallBookingBhatPara.Infrastructure.Service.User
{
    public class HallAvailMasterService : Repository<hall_availability_details>, IHallAvailMasterRepository
    {
        private readonly ApplicationDbContext _db;
        public HallAvailMasterService(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<bool> UpdateAsync(UpdateHallAvailableDTO model)
        {
            var HallAvail = await _db.hall_availability_details.FindAsync(model.HallId);
            if (HallAvail == null)
                return false;

            HallAvail.category_id_fk = model.CategoryId;
            HallAvail.hall_id_fk = model.SubcategoryId;
            HallAvail.hall_availability_from_date = model.AvailableFrom;
            HallAvail.hall_availability_to_date = model.AvailableTo;
            HallAvail.rate = model.ProposedRate;
            HallAvail.security_money = model.SecurityMoney;


            _db.hall_availability_details.Update(HallAvail);

            var result = await _db.SaveChangesAsync();
            return result > 0;
        }
    }
}
