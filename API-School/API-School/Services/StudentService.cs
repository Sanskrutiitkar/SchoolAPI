using API_School.DTO;
using API_School.Models;
using API_School.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API_School.Services
{
    public class StudentService:IStudentService
    {
        private readonly IMapper _mapper;
        private readonly IStudentRepo _studentRepo;
        public StudentService(IStudentRepo studentRepo, IMapper mapper)
        {
            _studentRepo = studentRepo;
            _mapper = mapper;
        }

        public async Task<IActionResult> AddStudent(StudentPostDto student)
        {
            Student MapedStudent = _mapper.Map<Student>(student);
            MapedStudent.StudentAge = CalculateAge(student.BirthDate);
            return await _studentRepo.AddStudent(MapedStudent);
        }

        public async Task<IActionResult> DeleteStudent(int StudentID)
        {
            return await _studentRepo.DeleteStudent(StudentID);
        }

        public async Task<IEnumerable<StudentRequestDto>> GetAllStudents()
        {
            IEnumerable<Student> students = await _studentRepo.GetAllStudents();
            return _mapper.Map<IEnumerable<StudentRequestDto>>(students); 

        }

        public async Task<ActionResult<PagedResponse<StudentRequestDto>>> GetSearchedStudents(string search, int pageNumber, int pageSize)
        {
            var pagedResponse = await _studentRepo.GetSearchedStudents(search, pageNumber, pageSize);
            return pagedResponse;
        }

        public async Task<IActionResult> UpdateStudent(int id, StudentPostDto student)
        {
            Student MapedStudent = _mapper.Map<Student>(student);
           MapedStudent.StudentAge = CalculateAge(student.BirthDate);
            return await _studentRepo.UpdateStudent(id, MapedStudent);
        }
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
