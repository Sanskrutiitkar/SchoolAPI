using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SchoolProject.Api.DTOs;
using SchoolProject.Api.Exceptions;
using SchoolProject.Api.Filter;
using SchoolProject.Api.GlobalException;
using SchoolProject.Api.Validators;
using SchoolProject.Buisness.Models;
using SchoolProject.Buisness.Repository;
using SchoolProject.Buisness.Services;
using Swashbuckle.AspNetCore.Annotations;


namespace SchoolProject.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [ServiceFilter(typeof(ModelValidationFilter))] 
    [ServiceFilter(typeof(APILoggingFilter))] 
    
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
    /// <summary>
    /// Retrieves a list of all students.
    /// </summary>
    /// <remarks>
    /// This endpoint returns a collection of student data transfer objects (DTOs).
    /// If no students are found, an empty list will be returned.
    /// </remarks>
    /// <returns>A list of <see cref="StudentRequestDto"/> objects.</returns>
    /// <response code="200">Returns the list of students or an empty list if no students are found</response>
    /// <response code="404">If no students are found in the database</response>
    [HttpGet]
    [SwaggerOperation(Summary = "Get all students", Description = "Retrieves a list of all students.")]
    [ProducesResponseType(typeof(IEnumerable<StudentRequestDto>), StatusCodes.Status200OK)]
    [Authorize(Roles ="Admin,Teacher")] 
        public async Task<ActionResult<StudentRequestDto>> Get()
        {
            var students = await _studentRepo.GetAllStudents();
            var dtoResponse = _mapper.Map<IEnumerable<StudentRequestDto>>(students);
            if(dtoResponse.Count()==0){
                return NotFound(dtoResponse);
            }
                    
            return Ok(dtoResponse);
        }

        /// <summary>
        /// Retrieves a student by their ID.
        /// </summary>
        /// <param name="id">The ID of the student to retrieve.</param>
        /// <remarks>
        /// This endpoint returns a single student data transfer object (DTO).
        /// If the student is not found, a 404 Not Found status will be returned.
        /// </remarks>
        /// <returns>A <see cref="StudentRequestDto"/> object representing the student.</returns>
        /// <response code="200">Returns the student DTO if found</response>
        /// <response code="404">If no student is found with the specified ID</response>
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get student by ID", Description = "Retrieves a student by their ID.")]
        [ProducesResponseType(typeof(StudentRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Authorize(Roles ="Admin,Teacher")]
        public async Task<ActionResult<StudentRequestDto>> GetStudentById(int id)
        {
            var student = await _studentRepo.GetStudentById(id);
            if (student == null)
            {
                return NotFound(ExceptionMessages.StudentNotFound);
            }

            var dtoResponse = _mapper.Map<StudentRequestDto>(student);
            return Ok(dtoResponse);
        }

        /// <summary>
        /// Creates a new student.
        /// </summary>
        /// <param name="studentDto">The student data transfer object containing the student's details.</param>
        /// <remarks>
        /// This endpoint adds a new student to the system. If a duplicate entry is detected,
        /// a 409 Conflict status will be returned with an error message.
        /// </remarks>
        /// <returns>A <see cref="StudentRequestDto"/> object representing the created student.</returns>
        /// <response code="200">Returns the created student DTO</response>
        /// <response code="409">If a duplicate entry is detected</response>
        [HttpPost]
        [SwaggerOperation(Summary = "Create a new student", Description = "Adds a new student to the system.")]
        [ProducesResponseType(typeof(StudentRequestDto), StatusCodes.Status200OK)]   
        [Authorize(Roles ="Admin")]   
        public async Task<ActionResult<StudentRequestDto>> Post([FromBody] StudentPostDto studentDto)
        {
            
            var mappedStudent = _mapper.Map<Student>(studentDto);
            
            mappedStudent.StudentAge = _studentService.CalculateAge(studentDto.BirthDate);
            Student isDuplicate = await _studentRepo.CheckDuplicate(mappedStudent);
            if(isDuplicate == null){
                var returnStudent = await _studentRepo.AddStudent(mappedStudent);
                var  returnMappedStudent= _mapper.Map<StudentRequestDto>(returnStudent);
                return Ok(returnMappedStudent);
            }
            throw new DuplicateEntryException(ExceptionMessages.DuplicateEntry);

        }

       /// <summary>
        /// Updates an existing student by their ID.
        /// </summary>
        /// <param name="id">The ID of the student to update.</param>
        /// <param name="studentDto">The updated student data transfer object containing the student's new details.</param>
        /// <remarks>
        /// This endpoint updates the details of an existing student. If the student is not found,
        /// a 404 Not Found status will be returned.
        /// </remarks>
        /// <returns>A <see cref="StudentRequestDto"/> object representing the updated student.</returns>
        /// <response code="200">Returns the updated student DTO</response>
        /// <response code="404">If no student is found with the specified ID</response>
        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Update an existing student", Description = "Updates a student's details by their ID.")]
        [ProducesResponseType(typeof(StudentRequestDto), StatusCodes.Status200OK)]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<StudentRequestDto>> Put(int id, [FromBody] UpdateStudentDto studentDto)
        {

            var existingStudent = await _studentRepo.GetStudentById(id);
            if (existingStudent == null)
            {
                return NotFound (ExceptionMessages.StudentNotFound); 
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

        /// <summary>
        /// Deletes a student by their ID.
        /// </summary>
        /// <param name="id">The ID of the student to delete.</param>
        /// <remarks>
        /// This endpoint deletes an existing student from the system. If the student is not found
        /// or is already inactive, a 404 Not Found status will be returned.
        /// </remarks>
        /// <response code="200">Indicates that the student was successfully deleted</response>
        /// <response code="404">If no student is found with the specified ID or if the student is inactive</response>
        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Delete a student", Description = "Deletes a student by their ID.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingStudent = await _studentRepo.GetStudentById(id);
            if (existingStudent == null || !existingStudent.IsActive)
            {
                return NotFound(ExceptionMessages.StudentNotFound);
            }
         
            await _studentRepo.DeleteStudent(id);
            return Ok();
        }

        /// <summary>
        /// Searches for students based on a search term and supports pagination.
        /// </summary>
        /// <param name="search">The search term to filter students by (optional).</param>
        /// <param name="pageNumber">The page number for pagination (default is 1).</param>
        /// <param name="pageSize">The number of records per page (default is 10, maximum is 100).</param>
        /// <remarks>
        /// This endpoint returns a paginated list of students matching the search criteria.
        /// If the page number is less than 1 or the page size is invalid, a PagedResponseException will be thrown.
        /// </remarks>
        /// <returns>A <see cref="PagedResponse{StudentRequestDto}"/> containing the paginated student data.</returns>
        /// <response code="200">Returns the paginated list of students</response>
        /// <response code="400">If the pagination parameters are invalid</response>
        [HttpGet("search")]
        [SwaggerOperation(Summary = "Search for students", Description = "Retrieves a paginated list of students based on a search term.")]
        [ProducesResponseType(typeof(PagedResponse<StudentRequestDto>), StatusCodes.Status200OK)]
        [Authorize(Roles ="Admin,Teacher")]

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