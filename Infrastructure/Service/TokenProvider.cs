using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.DTO;
using HallBookingBhatPara.Domain.Utility;

namespace HallBookingBhatPara.Infrastructure.Repository
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.AccessToken);
        }

        public TokenDTO GetToken()
        {
            try
            {
                bool hasAccessToken = _contextAccessor.HttpContext.Request.Cookies.TryGetValue(SD.AccessToken, out string accessToken);
                TokenDTO tokenDTO = new()
                {
                    AccessToken = accessToken
                };
                return hasAccessToken ? tokenDTO : null;
            }
            catch (Exception)
            {

                return null;
            }
        }

        public void SetToken(string AccessToken)
        {
            var cookieOptions = new CookieOptions { Expires = DateTime.UtcNow.AddDays(1) };
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.AccessToken, AccessToken, cookieOptions);
            _contextAccessor.HttpContext.Items["AccessToken"] = AccessToken;
        }
    }
}
