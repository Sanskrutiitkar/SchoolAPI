
using SchoolProject.Buisness.Models;

namespace SchoolProject.Api.DTOs
{
    public class StudentPostDto
    {
        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; }= string.Empty;

        public string StudentEmail { get; set; }= string.Empty;

        public string StudentPhone { get; set; }= string.Empty;

        public DateTime BirthDate { get; set; }

        public Gender StudentGender { get; set; }
   
    }
}