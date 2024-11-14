
using Microsoft.AspNetCore.Http;
using SchoolApi.Core.Business.Models;

namespace SchoolApi.Core.Business.GlobalException
{
     public class DuplicateEntryException : CustomException
    {
    public DuplicateEntryException(string message)
        : base(StatusCodes.Status409Conflict, message) { }
    }
}