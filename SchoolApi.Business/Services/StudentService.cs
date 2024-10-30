
using SchoolApi.Business.Models;
using SchoolApi.Business.Repository;


namespace SchoolApi.Business.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepo _studentRepo;

        public StudentService(IStudentRepo studentRepo)
        {
            _studentRepo = studentRepo;
        }

        // public async Task<Student> AddStudent(Student student)
        // {
        //     return await _studentRepo.AddStudent(student);
        // }

        // public async Task DeleteStudent(int studentID)
        // {
        //      await _studentRepo.DeleteStudent(studentID);
        // }

        // public async Task<IEnumerable<Student>> GetAllStudents()
        // {
        //     return await _studentRepo.GetAllStudents();
        // }

        // public async Task<PagedResponse<Student>> GetSearchedStudents(string search, int pageNumber, int pageSize)
        // {
        //     return await _studentRepo.GetSearchedStudents(search, pageNumber, pageSize);
        // }

        // public async Task UpdateStudent(int id, Student student)
        // {
        //      await _studentRepo.UpdateStudent(id, student);
        // }
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
