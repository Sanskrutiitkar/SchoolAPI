
using Microsoft.AspNetCore.Http;
using SchoolApi.Core.Business.Models;

namespace SchoolApi.Core.Business.GlobalException
{
     public class PagedResponseException : CustomException
    {
        public PagedResponseException(string message)
            : base(StatusCodes.Status400BadRequest, message) { }
    }
}