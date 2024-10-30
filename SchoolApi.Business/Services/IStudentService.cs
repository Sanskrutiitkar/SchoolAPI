
using SchoolApi.Business.Models;


namespace SchoolApi.Business.Services
{
    public interface IStudentService
    {
        // Task<Student> AddStudent(Student student);
        // Task DeleteStudent(int studentID);
        // Task<IEnumerable<Student>> GetAllStudents();
        // Task<PagedResponse<Student>> GetSearchedStudents(string search, int pageNumber, int pageSize);
        // Task UpdateStudent(int id, Student student);
        public int CalculateAge(DateTime birthDate);
    }
}
