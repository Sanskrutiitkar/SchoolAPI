

namespace SchoolProject.Buisness.Models
{
    public class Course
    {
    public int CourseId { get; set; }

    public string CourseName { get; set; } = string.Empty;

    public string InstructorName { get; set; } = string.Empty;

    public List<Student> Students { get; set; } = new List<Student>();
    }
}