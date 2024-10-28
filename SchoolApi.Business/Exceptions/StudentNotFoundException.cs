using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace SchoolApi.Business.Exceptions
{
    public class StudentNotFoundException : Exception
    {
        public StudentNotFoundException(string message) : base(message) { }
    }
}
