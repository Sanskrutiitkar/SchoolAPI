using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using UserProject.Api.DTOs;
using UserProject.Api.GlobalException;

namespace UserProject.Api.Exceptions
{
    public class CustomExceptionHandler:IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if(exception is DuplicateEntryException)
            {
                var response = new ErrorDetails()
                {
                    StatusCode = StatusCodes.Status409Conflict,
                    Message = "This email already exists",
                    ExceptionMessage = exception.Message
                };
                httpContext.Response.StatusCode=StatusCodes.Status409Conflict;
                await httpContext.Response.WriteAsJsonAsync(response);
                return true;
            }
              return false;
        }   
        
    }
}