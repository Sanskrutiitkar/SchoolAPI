using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.Api.Constants
{
   public class ExceptionMessages
    {
        public const string UserNotFound= "Student with this id not found";
        public const string AlreadyInactive= "Student with this id is already inactive";
        public const string AlreadyExists = "User with this email already exists";
        public const string InvalidCredentials="Invalid EmailId or Password";
        
    }
}