using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.Api.DTOs
{
    public class ValidationError
    {
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public string ExceptionMessage { get; set; }
        public IDictionary<string, List<string>> Errors { get; set; }
    }
}