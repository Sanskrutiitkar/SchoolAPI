using System.Text.Json.Serialization;

namespace API_School.Models
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
        //[JsonConverter(typeof(JsonStringEnumConverter))]

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
