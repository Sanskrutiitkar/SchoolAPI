
using SchoolApi.Business.Models;


namespace SchoolApi.Business.Services
{
    public interface IStudentService
    {
        
        public int CalculateAge(DateTime birthDate);
    }
}
