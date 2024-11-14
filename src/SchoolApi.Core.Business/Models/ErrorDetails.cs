using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApi.Core.Business.Models
{
    public class ErrorDetails
    {
         public int StatusCode { get; set; }
        public string ExceptionMessage { get; set; } = string.Empty;
    }
}