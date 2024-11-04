using AutoMapper;
using Bogus;
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

        public ControllerTest()
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new StudentAutoMapperProfile()); // Add your mapping profile here
            });

            _mapper = mappingConfig.CreateMapper();
           // _mapper = _mapper;
            _mockStudentService = new Mock<IStudentService>();
            _mockStudentRepo = new Mock<IStudentRepo>();
            _controller = new StudentController(_mockStudentService.Object, _mapper, _mockStudentRepo.Object);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WithListOfStudents()
        {
            // Arrange
            var students = new List<Student> { new Student { StudentId = 1, FirstName = "John" } };
            _mockStudentRepo.Setup(repo => repo.GetAllStudents()).ReturnsAsync(students);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<StudentRequestDto>>(okResult.Value);
            Assert.Single(returnValue);
        }

        [Fact]
        public async Task GetStudentById_ReturnsOkResult_WhenStudentExists()
        {
            // Arrange
            var student = new Student { StudentId = 1, FirstName = "John" };
            _mockStudentRepo.Setup(repo => repo.GetStudentById(1)).ReturnsAsync(student);

            // Act
            var result = await _controller.GetStudentById(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<StudentRequestDto>(okResult.Value);
        }

        [Fact]
        public async Task GetStudentById_ReturnsNotFound_WhenStudentDoesNotExist()
        {
            // Arrange
            _mockStudentRepo.Setup(repo => repo.GetStudentById(It.IsAny<int>())).ReturnsAsync((Student)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.GetStudentById(1));
        }
        [Fact]
        public async Task Post_ReturnsOkResult_WhenValidDataProvided()
        {
            // Arrange
            var studentDto = new StudentPostDto { FirstName = "Jane", BirthDate = DateTime.Now.AddYears(-20) };

            // Act
            var result = await _controller.Post(studentDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(studentDto, okResult.Value);
        }    

        [Fact]
        public async Task Put_ReturnsOkResult_WhenUpdatingExistingStudent()
        {
            // Arrange
            var existingStudent = new Student { StudentId = 1, FirstName = "John" };
            var updateDto = new UpdateStudentDto { FirstName = "Johnny" };

            _mockStudentRepo.Setup(repo => repo.GetStudentById(1)).ReturnsAsync(existingStudent);

            // Act
            var result = await _controller.Put(1, updateDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedStudent = Assert.IsType<Student>(okResult.Value);
            Assert.Equal("Johnny", updatedStudent.FirstName);
        }
        [Fact]
        public async Task Put_ReturnsNotFound_WhenUpdatingNonExistingStudent()
        {
            // Arrange
            _mockStudentRepo.Setup(repo => repo.GetStudentById(It.IsAny<int>())).ReturnsAsync((Student)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _controller.Put(1, new UpdateStudentDto()));
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenDeletingExistingStudent()
        {
            // Arrange
            var existingStudent = new Student { StudentId = 1, IsActive = true };

            _mockStudentRepo.Setup(repo => repo.GetStudentById(1)).ReturnsAsync(existingStudent);

            // Act
            var result = await _controller.Delete(1);

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
        public async Task StudentsSearch_ReturnsPagedResponse_WhenSearchingStudents()
        {
            // Arrange
             var pagedResponse = new PagedResponse<Student>(
                new List<Student> { new Student { StudentId = 1, FirstName = "Abhishek", LastName = "Nyamati", StudentEmail = "abhishek@gmail.com", StudentGender = (Gender)1, StudentPhone = "1234567890", BirthDate = new DateTime(2000, 1, 1), StudentAge = 22, IsActive = true } },
                pageNumber: 1,
                pageSize: 5,
                totalRecords: 1
            );

            _mockStudentRepo.Setup(repo => repo.GetSearchedStudents(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(pagedResponse);

            // Act
            var result = await _controller.StudentsSearch("john");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<PagedResponse<StudentRequestDto>>(okResult.Value);
            Assert.Single<StudentRequestDto>(returnValue.Data);
        }

    }
}

