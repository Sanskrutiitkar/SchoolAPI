

namespace SchoolProject.Buisness.Services
{
    public class StudentService:IStudentService
    {
        public int CalculateAge(DateTime birthDate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthDate.Year;
            if (birthDate > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
        
    }
}