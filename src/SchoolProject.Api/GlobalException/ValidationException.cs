using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Api.GlobalException
{
    public class ValidationException:Exception
    {
         public ValidationException(string message):base(message){
            
        }

        
    }
}