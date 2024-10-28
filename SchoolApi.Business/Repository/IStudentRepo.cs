
using SchoolApi.Business.Models;


namespace SchoolApi.Business.Repository
{
    public interface IStudentRepo
    {
        Task<Student> AddStudent(Student student);
        Task DeleteStudent(int studentID);
        Task<IEnumerable<Student>> GetAllStudents();
        Task UpdateStudent(int id, Student student);
        Task<PagedResponse<Student>> GetSearchedStudents(string search, int pageNumber, int pageSize);
    }
}
