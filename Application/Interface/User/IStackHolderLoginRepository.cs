using HallBookingBhatPara.Domain.Entities;

namespace HallBookingBhatPara.Application.Interface.User
{
    public interface IStackHolderLoginRepository : IRepository<hall_stake_holder_login>
    {
        Task<bool> UpdatePasswordAsync(string Password, long UserId, string EmailId,string EncriptPass);
    }
}
