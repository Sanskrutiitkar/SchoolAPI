using AutoMapper;
using Bogus;
using FluentValidation.AspNetCore;
using FluentValidation.TestHelper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolApi.APi.Exceptions;
using SchoolApi.API.Controllers;
using SchoolApi.API.DTOs;
using SchoolApi.API.Exceptions;
using SchoolApi.API.Mapper;
using SchoolApi.API.Validators;
using SchoolApi.Business.Models;
using SchoolApi.Business.Repository;
using SchoolApi.Business.Services;
using System;
using System.Dynamic;

namespace SchoolApi.Test
{
    public class ControllerTest
    {

        private readonly Mock<IStudentService> _mockStudentService;
        private readonly Mock<IStudentRepo> _mockStudentRepo;
        private readonly IMapper _mapper; 
        private readonly StudentController _controller;
        private readonly Faker<Student> _studentFaker;
        private readonly Faker<StudentPostDto> _addStudentDtoFaker;
        private readonly Faker<UpdateStudentDto> _studentUpdateFaker;
        private readonly StudentValidator _validator;
        public ControllerTest()
        {
            _validator= new StudentValidator();
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new StudentAutoMapperProfile()); 
            });

            _mapper = mappingConfig.CreateMapper();         
            _mockStudentService = new Mock<IStudentService>();
            _mockStudentRepo = new Mock<IStudentRepo>();
            _controller = new StudentController(_mockStudentService.Object, _mapper, _mockStudentRepo.Object);

            _studentFaker = new Faker<Student>()
                .RuleFor(s => s.StudentId, f => f.IndexFaker + 1)
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                .RuleFor(s => s.LastName, f => f.Name.LastName())
                .RuleFor(s => s.StudentEmail, f => f.Internet.Email())
               .RuleFor(s => s.StudentPhone, f =>
               {
                   var firstDigit = f.Random.Int(7, 9);
                   var remainingDigits = f.Random.Number(100000000, 999999999);
                   return $"{firstDigit}{remainingDigits}";
               })
                .RuleFor(s => s.BirthDate, f => f.Date.Past(10))
                .RuleFor(s => s.StudentGender, f => f.PickRandom<Gender>())
                .RuleFor(s => s.StudentAge, f => DateTime.Now.Year - f.Date.Past(10).Year);

            _addStudentDtoFaker = new Faker<StudentPostDto>()
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                .RuleFor(s => s.LastName, f => f.Name.LastName())
                .RuleFor(s => s.StudentEmail, f => f.Internet.Email())
                .RuleFor(s => s.StudentPhone, f =>
                {
                    var firstDigit = f.Random.Int(7, 9);
                    var remainingDigits = f.Random.Number(100000000, 999999999);
                    return $"{firstDigit}{remainingDigits}";
                })
                .RuleFor(s => s.BirthDate, f => f.Date.Past(20))
                .RuleFor(s => s.StudentGender, f => f.PickRandom<Gender>());

                _studentUpdateFaker = new Faker<UpdateStudentDto>()
               .RuleFor(s => s.FirstName, f => f.Name.FirstName())
               .RuleFor(s => s.LastName, f => f.Name.LastName())
               .RuleFor(s => s.StudentEmail, f => f.Internet.Email())
               .RuleFor(s => s.StudentPhone, f =>
               {
                   var firstDigit = f.Random.Int(7, 9);
                   var remainingDigits = f.Random.Number(100000000, 999999999);
                   return $"{firstDigit}{remainingDigits}";
               })
                .RuleFor(s => s.StudentGender, f => f.PickRandom<Gender>())
               .RuleFor(s => s.BirthDate, f => f.Date.Past(20, null));
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WithListOfStudents()
        {
            var students = _studentFaker.Generate(5);
            _mockStudentRepo.Setup(repo => repo.GetAllStudents()).ReturnsAsync(students);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedStudents = Assert.IsAssignableFrom<List<StudentRequestDto>>(okResult.Value);
            Assert.Equal(5, returnedStudents.Count());
        }

        [Fact]
        public async Task GetStudentById_ReturnsOkResult_WhenStudentExists()
        {
            // Arrange
            var student = _studentFaker.Generate();
            _mockStudentRepo.Setup(repo => repo.GetStudentById(student.StudentId)).ReturnsAsync(student);

            // Act
            var result = await _controller.GetStudentById(student.StudentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var foundStudent = Assert.IsType<StudentRequestDto>(okResult.Value);
            Assert.Equal(student.StudentId, foundStudent.StudentId);
        }

        [Fact]
        public async Task GetStudentById_ReturnsNotFound_WhenStudentDoesNotExist()
        {
            // Arrange
            _mockStudentRepo.Setup(repo => repo.GetStudentById(It.IsAny<int>())).ReturnsAsync((Student)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _controller.GetStudentById(1));
            Assert.Equal(ExceptionMessages.StudentNotFound, exception.Message);
        }
        [Fact]
        public async Task AddStudent_ShouldReturnOkResult_WhenValid_BestCase()
        {
            //var studentPostDTO = _addStudentDtoFaker.Generate();
            //var student = _mapper.Map<Student>(studentPostDTO);
            var student = _studentFaker.Generate();
            var dto = _mapper.Map<StudentPostDto>(student);

            _mockStudentRepo.Setup(repository => repository.AddStudent(It.IsAny<Student>())).ReturnsAsync(student);

            var result = await _controller.Post(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<StudentPostDto>(okResult.Value);
            Assert.Equal(student.FirstName, returnValue.FirstName);
            Assert.Equal(student.LastName, returnValue.LastName);
            Assert.Equal(student.StudentEmail, returnValue.StudentEmail);
            Assert.Equal(student.StudentPhone, returnValue.StudentPhone);
            Assert.Equal(student.BirthDate, returnValue.BirthDate);
           // Assert.Equal(student.StudentAge, returnValue.StudentAge);
        }
        [Fact]
        public async Task AddStudent_ShouldReturnBadRequest_WhenInvalid_WorstCase()
        {
            var studentPostDTO = _addStudentDtoFaker.Generate();
            studentPostDTO.FirstName = "";
            studentPostDTO.LastName = "";
            var student = _mapper.Map<Student>(studentPostDTO);


            var validator = new StudentValidator();
            var validationResult = validator.TestValidate(studentPostDTO);
            validationResult.ShouldHaveAnyValidationError();
            validationResult.AddToModelState(_controller.ModelState, null);

            var result = await _controller.Post(studentPostDTO);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = badRequestResult.Value as ValidationProblemDetails;

            Assert.NotNull(errors);
            
            Assert.Contains("FirstName", errors.Errors.Keys);
            Assert.Contains("LastName", errors.Errors.Keys);
            


        }

        [Fact]
        public async Task Put_ReturnsOkResult_WhenUpdatingExistingStudent()
        {
            var studentUpdateDTO = _studentUpdateFaker.Generate();

            var student = _studentFaker.Generate();
            student.StudentId = 1;

            _mockStudentRepo.Setup(repository => repository.GetStudentById(1)).ReturnsAsync(student);
            _mockStudentRepo.Setup(repository => repository.UpdateStudent(It.IsAny<Student>())).ReturnsAsync(student);

            var result = await _controller.Put(1, studentUpdateDTO);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<StudentRequestDto>(okResult.Value);
            Assert.Equal(student.FirstName, returnValue.FirstName);
            Assert.Equal(student.LastName, returnValue.LastName);
            Assert.Equal(student.StudentEmail, returnValue.StudentEmail);
            Assert.Equal(student.StudentPhone, returnValue.StudentPhone);
            Assert.Equal(student.BirthDate, returnValue.BirthDate);
            Assert.Equal(student.StudentAge, returnValue.StudentAge);
        }
        [Fact]
        public async Task Put_ReturnsNotFound_WhenUpdatingNonExistingStudent()
        {
            var studentUpdateDTO = _studentUpdateFaker.Generate();

            _mockStudentRepo.Setup(repository => repository.GetStudentById(999)).Throws(new Exception(ExceptionMessages.StudentNotFound));
            var exception = await Assert.ThrowsAsync<Exception>(async () => await _controller.Put(999, studentUpdateDTO));
            Assert.Equal(ExceptionMessages.StudentNotFound, exception.Message);
        }

       


        [Fact]
        public async Task Delete_ReturnsOk_WhenDeletingExistingStudent()
        {
            // Arrange
            var student = _studentFaker.Generate();

            _mockStudentRepo.Setup(repo => repo.GetStudentById(student.StudentId)).ReturnsAsync(student);
            
            // Act
            var result = await _controller.Delete(student.StudentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Student deleted successfully", okResult.Value);
        }

        [Fact]
        public async Task Delete_ThrowsException_WhenDeletingNonExistingStudent()
        {
            // Arrange
            _mockStudentRepo.Setup(repo => repo.GetStudentById(It.IsAny<int>())).ReturnsAsync((Student)null);

            // Act & Assert
            var exception  = await Assert.ThrowsAsync<Exception>(() => _controller.Delete(1));
            Assert.Equal(ExceptionMessages.StudentNotFound, exception.Message);
        }

        [Fact]
        public async Task GetPagedStudents_ReturnsOk_WithValidPagination()
        {
            // Arrange
            var students = _studentFaker.Generate(5);
            var pagedResponse = new PagedResponse<Student>(students, 1, 5, 10);

            _mockStudentRepo.Setup(repo => repo.GetSearchedStudents("", 1, 5)).ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.StudentsSearch(pageNumber: 1, pageSize: 5);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedPagedResponse = Assert.IsType<PagedResponse<StudentRequestDto>>(okResult.Value);
            Assert.Equal(5, returnedPagedResponse.Data.Count());
            Assert.Equal(10, returnedPagedResponse.TotalRecords);
        }
        

        [Fact]
        public void Should_HaveError_When_FirstNameIsNull()
        {
            var model = _addStudentDtoFaker.Generate();
            model.FirstName = null;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.FirstName).WithErrorMessage("First name cannot be null.");
        }

        [Fact]
        public void Should_HaveError_When_FirstNameIsEmpty()
        {
            var model = _addStudentDtoFaker.Generate();
            model.FirstName = string.Empty;
            _validator.TestValidate(model).ShouldHaveValidationErrorFor(student => student.FirstName).WithErrorMessage("First name cannot be empty.");
        }

    }
}

