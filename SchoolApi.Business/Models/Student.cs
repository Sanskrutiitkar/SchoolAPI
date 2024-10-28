using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolApi.Business.Models
{
    public class Student
    {

        public int StudentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string StudentEmail { get; set; }

        public string StudentPhone { get; set; }

        public DateTime BirthDate { get; set; }

        public int StudentAge { get; set; }

        public Gender StudentGender { get; set; }

        public bool IsActive { get; set; } = true;
    }
    public enum Gender
    {
        MALE = 1,
        FEMALE = 2,
        OTHER = 3
    }
}

