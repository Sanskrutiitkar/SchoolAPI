using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.API.DTOs;
using SchoolApi.API.Exceptions;
using SchoolApi.Business.Models;
using SchoolApi.Business.Repository;
using SchoolApi.Business.Services;

namespace SchoolApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;
        private readonly IStudentRepo _studentRepo;

        public StudentController(IStudentService studentService, IMapper mapper, IStudentRepo studentRepo)
        {
            _studentService = studentService;
            _mapper = mapper;
            _studentRepo = studentRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var students = await _studentRepo.GetAllStudents();
            var dtoResponse = _mapper.Map<IEnumerable<StudentRequestDto>>(students);
            return Ok(dtoResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudentById(int id)
        {
            var student = await _studentRepo.GetStudentById(id);
            if (student == null)
            {
                throw new Exception(ExceptionMessages.StudentNotFound);
            }

            var dtoResponse = _mapper.Map<StudentRequestDto>(student);
            return Ok(dtoResponse);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StudentPostDto studentDto)
        {       
            var mappedStudent = _mapper.Map<Student>(studentDto);
            mappedStudent.StudentAge = _studentService.CalculateAge(studentDto.BirthDate);

            await _studentRepo.AddStudent(mappedStudent);
            return Ok(studentDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UpdateStudentDto studentDto)
        {
            
            var existingStudent = await _studentRepo.GetStudentById(id);
            if (existingStudent == null)
            {
                throw new Exception(ExceptionMessages.StudentNotFound); 
            }

         
            if (!string.IsNullOrEmpty(studentDto.FirstName))
                existingStudent.FirstName = studentDto.FirstName;

            if (!string.IsNullOrEmpty(studentDto.LastName))
                existingStudent.LastName = studentDto.LastName;

            if (!string.IsNullOrEmpty(studentDto.StudentEmail))
                existingStudent.StudentEmail = studentDto.StudentEmail;

            if (!string.IsNullOrEmpty(studentDto.StudentPhone))
                existingStudent.StudentPhone = studentDto.StudentPhone;

            if (studentDto.BirthDate.HasValue)
            {
                existingStudent.BirthDate = studentDto.BirthDate.Value;
                existingStudent.StudentAge = _studentService.CalculateAge(studentDto.BirthDate.Value);
            }

            
            var returnedStudent = await _studentRepo.UpdateStudent(existingStudent);
            var mappedStudent = _mapper.Map<StudentRequestDto>(returnedStudent);

            return Ok(mappedStudent);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingStudent = await _studentRepo.GetStudentById(id);
            if (existingStudent == null)
            {
                throw new Exception(ExceptionMessages.StudentNotFound);
            }

            if (!existingStudent.IsActive)
            {
                throw new InvalidOperationException(ExceptionMessages.AlreadyInactive);
            }

            await _studentRepo.DeleteStudent(id);
            return Ok("Student deleted successfully");
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResponse<StudentRequestDto>>> StudentsSearch(string search = "", int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                throw new Exception(ExceptionMessages.PaginationPageNumer);
            }

            if (pageSize <= 0 || pageSize > 100)
            {
                throw new Exception(ExceptionMessages.PaginationPageSize);
            }
            var pagedResponse = await _studentRepo.GetSearchedStudents(search, pageNumber, pageSize);

            var dtoResponse = pagedResponse.Data.Select(s => _mapper.Map<StudentRequestDto>(s)).ToList();

            return Ok(new PagedResponse<StudentRequestDto>(dtoResponse, pagedResponse.PageNumber, pagedResponse.PageSize, pagedResponse.TotalRecords));
        }
    }
}