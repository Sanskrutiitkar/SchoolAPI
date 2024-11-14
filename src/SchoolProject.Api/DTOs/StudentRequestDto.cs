using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolProject.Api.DTOs
{
    public class StudentRequestDto
    {
         public int StudentId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string StudentEmail { get; set; } = string.Empty;

        public string StudentPhone { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public int StudentAge { get; set; }

        public string StudentGender { get; set; }= string.Empty;
   
    }
}