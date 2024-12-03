

namespace SchoolProject.Buisness.Models
{
    public class Student
    {
        
        public int StudentId { get; set; }

        public string FirstName { get; set; }  = string.Empty;

        public string LastName { get; set; }  = string.Empty;

        public string StudentEmail { get; set; }  = string.Empty;

        public string StudentPhone { get; set; }  = string.Empty;

        public DateTime BirthDate { get; set; }

        public Gender StudentGender { get; set; }

        public bool IsActive { get; set; } = true;
        public List<Course> Courses { get; set; } = new List<Course>();
    }
    public enum Gender
    {
        MALE = 1,
        FEMALE = 2,
        OTHER = 3
    }
}