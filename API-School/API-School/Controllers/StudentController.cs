using API_School.DTO;
using API_School.Models;
using API_School.Services;
using API_School.Validators;
using AutoMapper;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API_School.Controllers
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
        public async Task<IEnumerable<StudentRequestDto>> Get()
        {
            return await _studentService.GetAllStudents();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] StudentPostDto studentDto)
        {
            StudentValidator validator = new StudentValidator();

            ValidationResult result = validator.Validate(studentDto);
            if (!result.IsValid)
            {
                foreach (var failure in result.Errors)
                {
                    return BadRequest("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
                }
            }

            return await _studentService.AddStudent(studentDto);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] StudentPostDto studentDto)
        {
            StudentValidator validator = new StudentValidator();

            ValidationResult result = validator.Validate(studentDto);
            if (!result.IsValid)
            {
                foreach (var failure in result.Errors)
                {
                    return BadRequest("Property " + failure.PropertyName + " failed validation. Error was: " + failure.ErrorMessage);
                }
            }

            return await _studentService.UpdateStudent(id, studentDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return await _studentService.DeleteStudent(id);
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<PagedResponse<StudentRequestDto>>> StudentsSearch(string search = "", int pageNumber = 1, int pageSize = 10)
        {
            var pagedResponse = await _studentService.GetSearchedStudents(search, pageNumber, pageSize);
            return Ok(pagedResponse);
        }
    }
}
