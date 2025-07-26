using FluentValidation.Results;
using System.Net;

namespace HallBookingBhatPara.Infrastructure.Service
{
    public static class ResponseService
    {
        public static Response<T> SuccessResponse<T>(T result)
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Result = result
            };
        }

        public static Response<T> NotFoundResponse<T>(string errorMessage)
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.NotFound,
                IsSuccess = false,
                ErrorMessages = new List<string> { errorMessage }
            };
        }

        public static Response<T> BadRequestResponse<T>(string errorMessage)
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.BadRequest,
                IsSuccess = false,
                ErrorMessages = new List<string> { errorMessage }
            };
        }

        public static Response<T> InternalServerResponse<T>(string errorMessage)
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.InternalServerError,
                IsSuccess = false,
                ErrorMessages = new List<string> { errorMessage }
            };
        }

        public static Response<T> FluentValidationErrorResponse<T>(IEnumerable<ValidationFailure> errors)
        {
            return new Response<T>
            {
                StatusCode = HttpStatusCode.BadRequest,
                IsSuccess = false,
                ErrorMessages = errors.Select(e => e.ErrorMessage).ToList()
            };
        }

        public static Response<T> ErrorResponse<T>(string errorMessage)
        {
            return ErrorResponse<T>(new[] { errorMessage });
        }

        public static Response<T> ErrorResponse<T>(IEnumerable<string> errorMessages)
        {
            return new Response<T>
            {
                IsSuccess = false,
                StatusCode = HttpStatusCode.InternalServerError,
                ErrorMessages = errorMessages.ToList(),
                Result = default
            };
        }
    }


    public class Response<T>
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        public bool IsSuccess { get; set; } = true;
        public List<string> ErrorMessages { get; set; } = new();
        public T Result { get; set; }
    }
}
