using SchoolApi.Business.Models;

namespace SchoolApi.API.DTOs
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
