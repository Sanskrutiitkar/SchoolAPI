using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Api.DTOs
{
    public class StudentRequestDto
    {
         public int StudentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string StudentEmail { get; set; }

        public string StudentPhone { get; set; }

        public DateTime BirthDate { get; set; }

        public int StudentAge { get; set; }

        public string StudentGender { get; set; }
   
    }
}