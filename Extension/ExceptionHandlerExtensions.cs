using Microsoft.AspNetCore.Diagnostics;

namespace HallBookingBhatPara.Extension
{
    public static class ExceptionHandlerExtensions
    {
        public static void ConfigureGlobalExceptionHandler(this WebApplication app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exception = context.Features.Get<IExceptionHandlerPathFeature>()?.Error;

                    if (exception != null)
                    {
                        var helper = context.RequestServices.GetRequiredService<ExceptionHandlingHelper>();
                        await helper.HandleExceptionAsync(context, exception);
                    }
                });
            });
        }
    }
}
