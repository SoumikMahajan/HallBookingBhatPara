using HallBookingBhatPara.Application.Interface;
using HallBookingBhatPara.Domain.DTO;
using HallBookingBhatPara.Domain.Utility;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace HallBookingBhatPara.Infrastructure.Repository
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IConfiguration _configuration;
        public TokenProvider(IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _contextAccessor = contextAccessor;
            _configuration = configuration;
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

        public void SetToken(string accessToken)
        {
            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(2), // 2 days as requested
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            };
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.AccessToken, accessToken, cookieOptions);
            _contextAccessor.HttpContext.Items["AccessToken"] = accessToken;
        }
        public bool IsTokenValid()
        {
            var token = GetToken();
            if (token?.AccessToken == null) return false;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["ApiSettings:Secret"]);

                tokenHandler.ValidateToken(token.AccessToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return validatedToken != null;
            }
            catch
            {
                return false;
            }
        }

        public UserClaims? GetUserClaims()
        {
            var token = GetToken();
            if (token?.AccessToken == null) return null;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(token.AccessToken);

                return new UserClaims
                {
                    Id = jwt.Claims.FirstOrDefault(x => x.Type == "nameid")?.Value ?? "",
                    Email = jwt.Claims.FirstOrDefault(x => x.Type == "email")?.Value ?? "",
                    Name = jwt.Claims.FirstOrDefault(x => x.Type == "unique_name")?.Value ?? "",
                    Roles = jwt.Claims.FirstOrDefault(x => x.Type == "role")?.Value ?? "",
                    RolesId = jwt.Claims.FirstOrDefault(x => x.Type == "RolesId")?.Value ?? "",
                    StackHolderId = jwt.Claims.FirstOrDefault(x => x.Type == "StackHolderId")?.Value ?? "",
                    ProfileImagePath = jwt.Claims.FirstOrDefault(x => x.Type == "ProfilePic")?.Value ?? ""
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
