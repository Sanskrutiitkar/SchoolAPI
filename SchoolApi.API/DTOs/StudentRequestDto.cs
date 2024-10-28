namespace SchoolApi.API.DTOs
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
