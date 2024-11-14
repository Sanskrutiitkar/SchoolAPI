using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApi.Core.Business.Constants
{
    public class ExceptionMessages
    {
        public const string ExpiredToken = "Token has expired.";
        public const string InvalidSignature = "Token has an invalid signature.";
        public const string InvalidFormat ="Token format is invalid.";
        public const string UnexpectedError = "An unexpected error occurred.";
        public const string UnauthorizedAccess = "Unauthorized access. Token validation failed.";
        public const string AccessForbidden = "Access forbidden. You do not have permission to access this resource.";
    }
}