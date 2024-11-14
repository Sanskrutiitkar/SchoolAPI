

namespace SchoolApi.Core.Business.Models
{
    public class ErrorDetails
    {
         public int StatusCode { get; set; }
        public string ExceptionMessage { get; set; } = string.Empty;
    }
}