using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using SchoolProject.Api.DTOs;
using SchoolProject.Api.GlobalException;
using FluentValidation.Results;
namespace SchoolProject.Api.Exceptions
{
    public class CustomExceptionHandler:IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
 
            if (exception is StudentNotFoundException)
            {
                await HandleExceptionAsync(httpContext, StatusCodes.Status404NotFound, "Not Found", exception.Message);
                return true;
            }
            else if (exception is DuplicateEntryException)
            {
                await HandleExceptionAsync(httpContext, StatusCodes.Status409Conflict, "Duplicate Entry", exception.Message);
                return true;
            }
            else if (exception is PagedResponseException)
            {
                await HandleExceptionAsync(httpContext, StatusCodes.Status400BadRequest, "Invalid values", exception.Message);
                return true;
            }

            await HandleExceptionAsync(httpContext, StatusCodes.Status500InternalServerError, "Internal server error", exception.Message);
            return false; 
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, int statusCode, string message, string exceptionMessage)
        {
            var response = new ErrorDetails()
            {
                StatusCode = statusCode,
                Message = message,
                ExceptionMessage = exceptionMessage
            };

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(response);
        }
    }
}
