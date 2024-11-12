using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;

namespace SchoolProject.Api.Filter
{
    public class APILoggingFilter: IActionFilter
    {
        private readonly Serilog.ILogger _logger;
 
        public APILoggingFilter()
        {
            _logger = Log.ForContext<APILoggingFilter>();
        }
 
        public void OnActionExecuting(ActionExecutingContext context)
        {
           
        }
 
        public void OnActionExecuted(ActionExecutedContext context)
        {
            var logEntry = new ApiLog
            {
                Method = context.HttpContext.Request.Method,
                Path = context.HttpContext.Request.Path,
                StatusCode = context.HttpContext.Response.StatusCode,
                Timestamp = DateTime.Now
            };
 
            _logger.Information("{@ApiLogEntry}", logEntry);
 
            if (context.Result is ObjectResult objectResult && objectResult.Value != null)
            {
                var returnedObject = objectResult.Value;
               
                _logger.Information(
                    "Returned Object: {@ReturnedObject}",
                    returnedObject
                );
            }
            if (context.Exception is Exception exception && exception.Message != null)
            {
                var returnedObject = exception.Message;
               
                _logger.Information(
                    "Returned Object: {@ReturnedObject}",
                    returnedObject
                );
            }
        }
    }

    internal class ApiLog
    {
        public string Method { get; set; }
        public PathString Path { get; set; }
        public int StatusCode { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
    
        
    