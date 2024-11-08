using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Api.GlobalException
{
    public class PagedResponseException:Exception
    {
         public PagedResponseException(string message):base(message){
            
        }
    }
}