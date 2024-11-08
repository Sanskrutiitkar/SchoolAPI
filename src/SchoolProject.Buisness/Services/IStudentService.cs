using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Buisness.Services
{
    public interface IStudentService
    {
        public int CalculateAge(DateTime birthDate);
  
    }
}