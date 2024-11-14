
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using SchoolApi.Core.Business.Constants;
using SchoolApi.Core.Business.Models;

namespace SchoolApi.Core.Business.Exceptions
{
    public class CustomExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is CustomException customException)
            {
                return await HandleExceptionAsync(httpContext, customException.StatusCode, exception.Message);
            }
            return await HandleExceptionAsync(httpContext, StatusCodes.Status500InternalServerError, ExceptionMessages.UnexpectedError);
        }

        private async Task<bool> HandleExceptionAsync(HttpContext httpContext, int statusCode, string exceptionMessage)
        {
            var response = new ErrorDetails()
            {
                StatusCode = statusCode,
                ExceptionMessage = exceptionMessage
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(response);
            return true;
        }
    }
}