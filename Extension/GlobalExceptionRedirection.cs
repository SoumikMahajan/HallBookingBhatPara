using Microsoft.AspNetCore.Mvc.Filters;

namespace HallBookingBhatPara.Extension
{
    public class GlobalExceptionRedirection : IAsyncExceptionFilter
    {
        private readonly ExceptionHandlingHelper _helper;

        public GlobalExceptionRedirection(ExceptionHandlingHelper helper)
        {
            _helper = helper;
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var exception = context.Exception;

            await _helper.HandleExceptionAsync(context.HttpContext, exception);

            context.ExceptionHandled = true;
        }
    }
}
