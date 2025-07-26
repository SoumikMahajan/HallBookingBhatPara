using HallBookingBhatPara.Infrastructure.Repository;
using HallBookingBhatPara.Infrastructure.Service;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Net;
using System.Security.Authentication;

namespace HallBookingBhatPara.Extension
{
    public class ExceptionHandlingHelper
    {
        private readonly LogService _logService;
        private readonly ITempDataDictionaryFactory _tempDataFactory;

        public ExceptionHandlingHelper(LogService logService, ITempDataDictionaryFactory tempDataFactory)
        {
            _logService = logService;
            _tempDataFactory = tempDataFactory;
        }

        public async Task HandleExceptionAsync(HttpContext httpContext, Exception exception)
        {
            await _logService.LogExceptionErrorAsync(exception);

            var (message, statusCode) = GetErrorMessageAndStatus(exception);
            var isAjax = httpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            if (isAjax)
            {
                httpContext.Response.StatusCode = statusCode;
                httpContext.Response.ContentType = "application/json";

                var errorResponse = ResponseService.ErrorResponse<object>(message);
                errorResponse.StatusCode = (HttpStatusCode)statusCode;
                errorResponse.Result = new
                {
                    redirectUrl = "/User/Login"
                };

                await httpContext.Response.WriteAsJsonAsync(errorResponse);
            }
            else
            {
                var tempData = _tempDataFactory.GetTempData(httpContext);

                var referer = httpContext.Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer))
                {
                    tempData["GlobalErrorMessage"] = message;
                }
                httpContext.Response.Redirect(!string.IsNullOrWhiteSpace(referer) ? referer : $"/Home/Error?message={Uri.EscapeDataString(message)}");
            }
        }

        public (string message, int statusCode) GetErrorMessageAndStatus(Exception exception)
        {
            return exception switch
            {
                AuthenticationException => ("You do not have permission to access this resource.", (int)HttpStatusCode.Forbidden),
                UnauthorizedAccessException => ("Unauthorized access.", (int)HttpStatusCode.Unauthorized),
                _ => (string.IsNullOrWhiteSpace(exception.Message) ? "An unknown error occurred" : exception.Message,
                     (int)HttpStatusCode.InternalServerError)
            };
        }
    }
}
