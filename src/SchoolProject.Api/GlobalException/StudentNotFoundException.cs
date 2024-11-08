using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Api.GlobalException
{
    public class StudentNotFoundException:Exception
    {
        public StudentNotFoundException(string message):base(message){
            
        }
    }
}