using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.API.DTOs;
using SchoolApi.API.Validators;
using SchoolApi.Business.Exceptions;
using SchoolApi.Business.Models;
using SchoolApi.Business.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SchoolApi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IMapper _mapper;

        public StudentController(IStudentService studentService, IMapper mapper)
        {
            _studentService = studentService;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var students = await _studentService.GetAllStudents();
                var dtoResponse = _mapper.Map<IEnumerable<StudentRequestDto>>(students);
                return Ok(dtoResponse); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StudentPostDto studentDto)
        {
            var validator = new StudentValidator();
            var result = validator.Validate(studentDto);

            if (!result.IsValid)
            {
                foreach (var failure in result.Errors)
                {
                    return BadRequest($"Property {failure.PropertyName} failed validation. Error was: {failure.ErrorMessage}");
                }
            }

            var mappedStudent = _mapper.Map<Student>(studentDto);
            mappedStudent.StudentAge = _studentService.CalculateAge(studentDto.BirthDate);

            try
            {
               var returnedStudent =  await _studentService.AddStudent(mappedStudent);
                return Ok(returnedStudent);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] StudentPostDto studentDto)
        {

            var validator = new StudentValidator();
            var result = validator.Validate(studentDto);

            if (!result.IsValid)
            {
                foreach (var failure in result.Errors)
                {
                    return BadRequest($"Property {failure.PropertyName} failed validation. Error was: {failure.ErrorMessage}");
                }
            }

            var mappedStudent = _mapper.Map<Student>(studentDto);
            mappedStudent.StudentAge = _studentService.CalculateAge(studentDto.BirthDate);

            try
            {
                await _studentService.UpdateStudent(id, mappedStudent);
                return Ok($"Student with ID {id} updated successfully");
            }
            catch (StudentNotFoundException ex)
            {
                return NotFound( ex.Message );
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _studentService.DeleteStudent(id);
                return Ok("Student deleted successfully");
            }
            catch (StudentNotFoundException ex)
            {
                return NotFound( ex.Message );
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> StudentsSearch(string search = "", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var pagedResponse = await _studentService.GetSearchedStudents(search, pageNumber, pageSize);

                var dtoResponse = pagedResponse.Data.Select(s => _mapper.Map<StudentRequestDto>(s)).ToList();

                return Ok(new PagedResponse<StudentRequestDto>(dtoResponse, pagedResponse.PageNumber, pagedResponse.PageSize, pagedResponse.TotalRecords));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

    }
}
