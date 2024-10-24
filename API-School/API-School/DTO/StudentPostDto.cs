using API_School.Models;

namespace API_School.DTO
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
