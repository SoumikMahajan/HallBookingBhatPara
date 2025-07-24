using HallBookingBhatPara.Domain.DTO;

namespace HallBookingBhatPara.Application.Interface
{
    public interface ITokenProvider
    {
        void SetToken(string AccessTOken);
        TokenDTO? GetToken();
        void ClearToken();
    }
}
