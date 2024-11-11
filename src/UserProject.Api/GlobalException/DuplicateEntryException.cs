using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.Api.GlobalException
{
     public class DuplicateEntryException:Exception
    {       
        public DuplicateEntryException(string message):base(message){
            
        }
    }
}
