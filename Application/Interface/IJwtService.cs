using HallBookingBhatPara.Domain.DTO;
using System.Security.Claims;

namespace HallBookingBhatPara.Application.Interface
{
    public interface IJwtService
    {
        string GenerateToken(UserClaims userClaims);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
