using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SchoolApi.Core.Business.GlobalException;
using SchoolApi.Core.Business.Models;

namespace SchoolApi.Core.Business.Exceptions
{
   public class CustomExceptionHandler:IExceptionHandler
    {
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is CustomException customException)
        {
            await HandleExceptionAsync(httpContext, customException.StatusCode, exception.Message);
            return true;
        }

        // else if (exception is SecurityTokenExpiredException )
        // {
        //     await HandleExceptionAsync(httpContext, StatusCodes.Status401Unauthorized, "Token has expired.");
        //     return true;
        // }
        // else if (exception is SecurityTokenInvalidSignatureException )
        // {
        //     await HandleExceptionAsync(httpContext, StatusCodes.Status401Unauthorized, "Token has an invalid signature.");
        //     return true;
        // }
        // else if (exception is SecurityTokenMalformedException )
        // {
        //     await HandleExceptionAsync(httpContext, StatusCodes.Status400BadRequest, "Token format is invalid.");
        //     return true;
        // }
        // else if (exception is UnauthorizedAccessException )
        // {
        //     await HandleExceptionAsync(httpContext, StatusCodes.Status401Unauthorized, "Unauthorized access.");
        //     return true;
        // }
        await HandleExceptionAsync(httpContext, StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        return false;

        }

         private async Task HandleExceptionAsync(HttpContext httpContext, int statusCode, string exceptionMessage)
        {
            var response = new ErrorDetails()
            {
                StatusCode = statusCode,
                ExceptionMessage =  exceptionMessage
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}