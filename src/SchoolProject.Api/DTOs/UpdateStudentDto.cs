using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchoolProject.Buisness.Models;

namespace SchoolProject.Api.DTOs
{
    public class UpdateStudentDto
    {
         public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? StudentEmail { get; set; }

        public string? StudentPhone { get; set; }

        public DateTime? BirthDate { get; set; }

        public Gender? StudentGender { get; set; }
    
    }
}