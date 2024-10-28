using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolApi.API.Controllers;
using SchoolApi.API.DTOs;
using SchoolApi.API.Validators;
using SchoolApi.Business.Exceptions;
using SchoolApi.Business.Models;
using SchoolApi.Business.Services;
using System.Dynamic;

namespace SchoolApi.Test
{
    public class ControllerTest
    {
       
        private readonly Mock<IStudentService> _studentServiceMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly StudentController _controller;

        public ControllerTest()
        {
            _studentServiceMock = new Mock<IStudentService>();
            _mapperMock = new Mock<IMapper>();
            _controller = new StudentController(_studentServiceMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Get_ShouldReturnOkResult_WithStudents()
        {
            // Arrange
            var students = new List<Student>
        {
            new Student { StudentId = 1, FirstName = "Sanskruti", LastName = "Itkar" }
        };
            _studentServiceMock.Setup(s => s.GetAllStudents()).ReturnsAsync(students);
            _mapperMock.Setup(m => m.Map<IEnumerable<StudentRequestDto>>(It.IsAny<IEnumerable<Student>>()))
                .Returns(new List<StudentRequestDto> { new StudentRequestDto { FirstName = "Sanskruti", LastName = "Itkar" } });

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<StudentRequestDto>>(okResult.Value);
            Assert.Single(returnValue);
        }
     
        [Fact]
        public async Task Delete_ShouldReturnOk_WhenStudentIsDeleted()
        {
            // Arrange
            _studentServiceMock.Setup(s => s.DeleteStudent(1)).Returns(Task.CompletedTask);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Student deleted successfully", okResult.Value);
        }

        [Fact]
        public async Task Delete_ShouldReturnBadRequest_WhenStudentIsAlreadyInactive()
        {
            // Arrange
            _studentServiceMock.Setup(s => s.DeleteStudent(1)).ThrowsAsync(new InvalidOperationException("Student is already inactive."));

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Student is already inactive.", badRequestResult.Value);
        }

        [Fact]
        public async Task StudentsSearch_ShouldReturnInternalServerError_OnException()
        {
            // Arrange
            _studentServiceMock.Setup(s => s.GetSearchedStudents("Sanskruti", 1, 10)).ThrowsAsync(new Exception("Some error"));

            // Act
            var result = await _controller.StudentsSearch("Sanskruti", 1, 10);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
        }

        //[Fact]
        //public async Task StudentsSearch_ShouldReturnOkResult_WithFilteredStudents()
        //{
        //    // Arrange
        //    var pagedResponse = new PagedResponse<Student>
        //    {
        //        Data = new List<Student> { new Student { FirstName = "Bruce" } },
        //        PageNumber = 1,
        //        PageSize = 10,
        //        TotalRecords = 1
        //    };
        //    _studentServiceMock.Setup(s => s.GetSearchedStudents("Bruce", 1, 10)).ReturnsAsync(pagedResponse);
        //    _mapperMock.Setup(m => m.Map<StudentRequestDto>(It.IsAny<Student>()))
        //        .Returns(new StudentRequestDto { FirstName = "Bruce" });

        //    // Act
        //    var result = await _controller.StudentsSearch("Bruce", 1, 10);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result);
        //    var response = Assert.IsAssignableFrom<PagedResponse<StudentRequestDto>>(okResult.Value);
        //    Assert.Single(response.Data);
        //}

        [Fact]
        public async Task Post_ShouldReturnOk_WhenStudentIsCreated()
        {
            // Arrange
            var studentDto = new StudentPostDto { FirstName = "Sandeep", LastName = "joshi", BirthDate = DateTime.Now.AddYears(-20) , StudentGender=(Gender)1};
            var mappedStudent = new Student { StudentId = 1, FirstName = "Sandeep", LastName = "joshi", StudentAge = 20 ,StudentGender = (Gender)1 };

            
            _studentServiceMock.Setup(s => s.AddStudent(It.IsAny<Student>())).ReturnsAsync(mappedStudent);
            _mapperMock.Setup(m => m.Map<Student>(It.IsAny<StudentPostDto>())).Returns(mappedStudent);

            // Act
            var result = await _controller.Post(studentDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Student>(okResult.Value); // Adjusted to check if it returns Student
            Assert.Equal(mappedStudent.FirstName, returnValue.FirstName);
            Assert.Equal(mappedStudent.LastName, returnValue.LastName);
        }

        [Fact]
        public async Task Post_ShouldReturnBadRequest_WhenValidationFails()
        {
            // Arrange
            var studentDto = new StudentPostDto { FirstName = "", LastName = "Itkar", BirthDate = DateTime.Now.AddYears(-20) }; // Invalid first name
            var validator = new StudentValidator();
            var result = validator.Validate(studentDto);

            // Act
            var actionResult = await _controller.Post(studentDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            Assert.Contains("Property FirstName failed validation", badRequestResult.Value.ToString());
        }


        [Fact]
        public async Task Put_ShouldReturnOk_WhenStudentIsUpdated()
        {
            // Arrange
            var studentDto = new StudentPostDto { FirstName = "Sandeep", LastName = "Itkar", BirthDate = DateTime.Now.AddYears(-20),StudentGender=(Gender)1 };
            var mappedStudent = new Student { StudentId = 1, FirstName = "Sandeep", LastName = "Itkar", StudentAge = 20,StudentGender=(Gender)1};

            _studentServiceMock.Setup(s => s.UpdateStudent(1, It.IsAny<Student>())).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<Student>(It.IsAny<StudentPostDto>())).Returns(mappedStudent);

            // Act
            var result = await _controller.Put(1, studentDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Student with ID 1 updated successfully", okResult.Value);
        }

        [Fact]
        public async Task Put_ShouldReturnNotFound_WhenStudentDoesNotExist()
        {
            // Arrange
            var studentDto = new StudentPostDto { FirstName = "Sandeep", LastName = "joshi", BirthDate = DateTime.Now.AddYears(-20), StudentGender=(Gender)1 };
            var mappedStudent = new Student { FirstName = "Sandeep", LastName = "joshi", StudentAge = 20 , StudentGender = (Gender)1 };

            _studentServiceMock.Setup(s => s.UpdateStudent(1, It.IsAny<Student>())).ThrowsAsync(new StudentNotFoundException("Student not found"));
            _mapperMock.Setup(m => m.Map<Student>(It.IsAny<StudentPostDto>())).Returns(mappedStudent);
            // Act
            var result = await _controller.Put(1, studentDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
           

            Assert.Equal("Student not found", notFoundResult.Value);
        }



    }
}

