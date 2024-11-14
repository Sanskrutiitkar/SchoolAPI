using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApi.Core.Business.Models
{
    public class CustomException: Exception
{
    public int StatusCode { get; }

    public CustomException(int statusCode, string message)
        : base(message)
    {
        StatusCode = statusCode;
    } 
        
    }
}