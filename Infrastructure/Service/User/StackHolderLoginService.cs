using HallBookingBhatPara.Application.Interface.User;
using HallBookingBhatPara.Domain.Entities;
using HallBookingBhatPara.Infrastructure.Data;
using HallBookingBhatPara.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;

namespace HallBookingBhatPara.Infrastructure.Service.User
{
	public class StackHolderLoginService : Repository<hall_stake_holder_login>, IStackHolderLoginRepository
	{
		private readonly ApplicationDbContext _db;
		public StackHolderLoginService(ApplicationDbContext db) : base(db)
		{
			_db = db;
		}
		public async Task<bool> UpdatePasswordAsync(string Password, long UserId, string EmailId, string EncriptPass)
		{
			var user = await _db.hall_stake_holder_login.FirstOrDefaultAsync(x => x.stake_details_id_fk == UserId && x.login_id == EmailId);
			if (user == null)
				return false;

			user.base_password = Password;
			user.login_password = EncriptPass;

			_db.hall_stake_holder_login.Update(user);

			var result = await _db.SaveChangesAsync();
			return result > 0;
		}
	}
}
