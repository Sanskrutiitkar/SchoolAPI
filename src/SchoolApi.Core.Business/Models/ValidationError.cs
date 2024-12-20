

namespace SchoolApi.Core.Business.Models
{
    public class ValidationError
    {
        public string? Message { get; set; } 
        public int StatusCode { get; set; }
        public string? ExceptionMessage { get; set; }
        public IDictionary<string, List<string>>? Errors { get; set; }
    }
}