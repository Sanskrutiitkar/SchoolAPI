
using SchoolProject.Buisness.Models;

namespace SchoolProject.Buisness.Repository
{
    public interface IStudentRepo
    {
        Task<Student?> GetStudentById(int id);
        Task<Student> AddStudent(Student student);
        Task DeleteStudent(int studentID);
        Task<IEnumerable<Student>> GetAllStudents();
        Task<Student> UpdateStudent(Student student);
        Task<PagedResponse<Student>> GetSearchedStudents(string search, int pageNumber, int pageSize);       
        Task<Student?> CheckDuplicate(Student student);
    
    }
}