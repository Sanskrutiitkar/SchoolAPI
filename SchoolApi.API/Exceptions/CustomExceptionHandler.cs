using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolApi.API.DTOs;
using System.Net;
using System.Text.Json;

namespace SchoolApi.APi.Exceptions
{
    public class CustomExceptionHandler : IMiddleware
    {
        private readonly ILogger<CustomExceptionHandler> _logger;

        public CustomExceptionHandler(ILogger<CustomExceptionHandler> logger)
        {
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var traceId = Guid.NewGuid();
            _logger.LogError($"TraceId: {traceId}, Exception: {exception.Message}, StackTrace: {exception.StackTrace}");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var errorDetails = new ErrorDetails()
            {
                TraceId = traceId,
                Message = "Internal Server Error from the custom middleware.",
                StatusCode = context.Response.StatusCode,
                Instance = context.Request.Path,
                ExceptionMessage = exception.Message
            };

            return context.Response.WriteAsJsonAsync(errorDetails);
        }
    }
}

  
