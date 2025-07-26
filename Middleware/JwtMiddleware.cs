using HallBookingBhatPara.Application.Interface;

namespace HallBookingBhatPara.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Resolve services from the request scope
            var tokenProvider = context.RequestServices.GetRequiredService<ITokenProvider>();
            var jwtService = context.RequestServices.GetRequiredService<IJwtService>();

            var token = tokenProvider.GetToken();

            if (token?.AccessToken != null)
            {
                var principal = jwtService.ValidateToken(token.AccessToken);
                if (principal != null)
                {
                    context.User = principal;
                }
                else
                {
                    // Token is invalid, clear it
                    tokenProvider.ClearToken();
                }
            }

            await _next(context);
        }
    }
}
