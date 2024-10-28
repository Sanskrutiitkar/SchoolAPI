using SchoolApi.Business.Models;
using System.Reflection;

namespace SchoolApi.API.DTOs
{
    public class StudentPostDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string StudentEmail { get; set; }

        public string StudentPhone { get; set; }

        public DateTime BirthDate { get; set; }

        public Gender StudentGender { get; set; }
    }
}
