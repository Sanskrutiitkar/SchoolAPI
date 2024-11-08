using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SchoolProject.Api.DTOs;
using SchoolProject.Api.Exceptions;
using SchoolProject.Api.Filter;
using SchoolProject.Api.GlobalException;
using SchoolProject.Api.Validators;
using SchoolProject.Buisness.Models;
using SchoolProject.Buisness.Repository;
using SchoolProject.Buisness.Services;


namespace SchoolProject.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ModelValidationFilter))] 
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
        public async Task<ActionResult<StudentRequestDto>> Get()
        {
            var students = await _studentRepo.GetAllStudents();
            if(students.Count()==0){
                throw new StudentNotFoundException(ExceptionMessages.StudentNotFound);
            }
            var dtoResponse = _mapper.Map<IEnumerable<StudentRequestDto>>(students);            
            return Ok(dtoResponse);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentRequestDto>> GetStudentById(int id)
        {
            var student = await _studentRepo.GetStudentById(id);
            if (student == null)
            {
                throw new StudentNotFoundException(ExceptionMessages.StudentNotFound);
            }

            var dtoResponse = _mapper.Map<StudentRequestDto>(student);
            return Ok(dtoResponse);
        }

        [HttpPost]
        public async Task<ActionResult<StudentRequestDto>> Post([FromBody] StudentPostDto studentDto)
        {
            
            var mappedStudent = _mapper.Map<Student>(studentDto);
            
            mappedStudent.StudentAge = _studentService.CalculateAge(studentDto.BirthDate);
            Student isDuplicate = await _studentRepo.CheckDuplicate(mappedStudent);
            if(isDuplicate != null){
                await _studentRepo.AddStudent(mappedStudent);
                var returnStudent = _mapper.Map<StudentRequestDto>(mappedStudent);
                return Ok(returnStudent);
            }
            throw new DuplicateEntryException(ExceptionMessages.DuplicateEntry);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult<StudentRequestDto>> Put(int id, [FromBody] UpdateStudentDto studentDto)
        {

            var existingStudent = await _studentRepo.GetStudentById(id);
            if (existingStudent == null)
            {
                throw new StudentNotFoundException(ExceptionMessages.StudentNotFound); 
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
            if (existingStudent == null || !existingStudent.IsActive)
            {
                throw new StudentNotFoundException(ExceptionMessages.StudentNotFound);
            }
         
            await _studentRepo.DeleteStudent(id);
            return Ok();
        }

        [HttpGet("search")]
        public async Task<ActionResult<PagedResponse<StudentRequestDto>>> StudentsSearch(string search = "", int pageNumber = 1, int pageSize = 10)
        {
            if (pageNumber < 1)
            {
                throw new PagedResponseException(ExceptionMessages.PaginationPageNumer);
            }

            if (pageSize <= 0 || pageSize > 100)
            {
                throw new PagedResponseException(ExceptionMessages.PaginationPageSize);
            }
            var pagedResponse = await _studentRepo.GetSearchedStudents(search, pageNumber, pageSize);

            var dtoResponse = pagedResponse.Data.Select(s => _mapper.Map<StudentRequestDto>(s)).ToList();

            return Ok(new PagedResponse<StudentRequestDto>(dtoResponse, pagedResponse.PageNumber, pagedResponse.PageSize, pagedResponse.TotalRecords));
        }
    }

 
}