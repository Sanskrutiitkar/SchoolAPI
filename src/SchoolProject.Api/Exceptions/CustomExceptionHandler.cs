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
            if(exception is StudentNotFoundException)
            {
                var response = new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    Message = "Not Found",
                    ExceptionMessage = exception.Message
                };

                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }
            else if(exception is DuplicateEntryException){
                var response = new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "Duplicate Entry",
                    ExceptionMessage = exception.Message
                };

                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }
            else if(exception is DuplicateEntryException){
                var response = new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status400BadRequest,
                    Message = "Invalid values",
                    ExceptionMessage = exception.Message
                };

                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }
             
            return false;
        }
    }
}