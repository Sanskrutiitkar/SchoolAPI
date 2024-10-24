using API_School.DTO;
using API_School.Models;
using API_School.Repository;
using API_School.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentApiTest
{
    [TestClass]
    public class StudentTest
    {
        private Mock<IStudentRepo> _studentRepoMock;
        private Mock<IMapper> _mapperMock;
        private StudentService _studentService;

        [TestInitialize]
        public void Setup()
        {
            _studentRepoMock = new Mock<IStudentRepo>();
            _mapperMock = new Mock<IMapper>();
            _studentService = new StudentService(_studentRepoMock.Object, _mapperMock.Object);
        }

        [TestMethod]
        public async Task AddStudent_ShouldCallAddStudentInRepo()
        {
            // Arrange
            var studentDto = new StudentPostDto { BirthDate = new DateTime(2000, 1, 1) };
            var mappedStudent = new Student { StudentAge = 24 };

            _mapperMock.Setup(m => m.Map<Student>(It.IsAny<StudentPostDto>()))
                .Returns(mappedStudent);
            _studentRepoMock.Setup(repo => repo.AddStudent(mappedStudent))
                .ReturnsAsync(new OkResult());

            // Act
            var result = await _studentService.AddStudent(studentDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(IActionResult));
            _studentRepoMock.Verify(repo => repo.AddStudent(mappedStudent), Times.Once);
        }

        [TestMethod]
        public async Task DeleteStudent_ShouldCallDeleteStudentInRepo()
        {
            // Arrange
            int studentId=1;
            _studentRepoMock.Setup(repo => repo.DeleteStudent(studentId))
                .ReturnsAsync(new OkObjectResult(new { message = "Student deleted succesfully" }));

            // Act
            var result = await _studentService.DeleteStudent(studentId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _studentRepoMock.Verify(repo => repo.DeleteStudent(studentId), Times.Once);
        }

        [TestMethod]
        public async Task DeleteStudent_ShouldCallDeleteStudentInRepo_InvalidId()
        {
            // Arrange
            int studentId = 999;
            _studentRepoMock.Setup(repo => repo.DeleteStudent(studentId))
                .ReturnsAsync(new BadRequestObjectResult(new { message = "Student with this id not found" }));

            // Act
            var result = await _studentService.DeleteStudent(studentId);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _studentRepoMock.Verify(repo => repo.DeleteStudent(studentId), Times.Once);
        }

        [TestMethod]
        public async Task GetAllStudents_ShouldReturnMappedStudents()
        {
            // Arrange
            var students = new List<Student> { new Student() };
            var studentDtos = new List<StudentRequestDto> { new StudentRequestDto() };

            _studentRepoMock.Setup(repo => repo.GetAllStudents())
                .ReturnsAsync(students);
            _mapperMock.Setup(m => m.Map<IEnumerable<StudentRequestDto>>(students))
                .Returns(studentDtos);

            // Act
            var result = await _studentService.GetAllStudents();

            // Assert
            Assert.AreEqual(studentDtos, result);
        }

        [TestMethod]
        public async Task GetAllStudents_ShouldReturnMappedStudents_Invalid()
        {
            // Arrange
            var students = new List<Student> {  };
            var studentDtos = new List<StudentRequestDto> {  };

            _studentRepoMock.Setup(repo => repo.GetAllStudents())
                .ReturnsAsync(students);
            _mapperMock.Setup(m => m.Map<IEnumerable<StudentRequestDto>>(students))
                .Returns(studentDtos);

            // Act
            var result = await _studentService.GetAllStudents();

            // Assert
            Assert.AreEqual(0, result.Count());

        }

        [TestMethod]
        public async Task UpdateStudent_ShouldCallUpdateStudentInRepo()
        {
            // Arrange
            int studentId = 1;
            var studentDto = new StudentPostDto { BirthDate = new DateTime(2000, 1, 1) };
            var mappedStudent = new Student { StudentAge = 24 }; 

            _mapperMock.Setup(m => m.Map<Student>(It.IsAny<StudentPostDto>()))
                .Returns(mappedStudent);

            _studentRepoMock.Setup(repo => repo.UpdateStudent(studentId, mappedStudent))
                .ReturnsAsync(new OkObjectResult(new { message = $"Student with updated succesfully" }));

            // Act
            var result = await _studentService.UpdateStudent(studentId, studentDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            _studentRepoMock.Verify(repo => repo.UpdateStudent(studentId, mappedStudent), Times.Once);
        }

        [TestMethod]
        public async Task UpdateStudent_ShouldCallUpdateStudentInRepo_Invalid()
        {
            // Arrange
            int studentId = 999;
            var studentDto = new StudentPostDto { BirthDate = new DateTime(2000, 1, 1) };
            var mappedStudent = new Student { StudentAge = 24 };

            _mapperMock.Setup(m => m.Map<Student>(It.IsAny<StudentPostDto>()))
                .Returns(mappedStudent);

            _studentRepoMock.Setup(repo => repo.UpdateStudent(studentId, mappedStudent))
                .ReturnsAsync(new BadRequestObjectResult(new { message = "Student With this id does not exists" }));

            // Act
            var result = await _studentService.UpdateStudent(studentId, studentDto);

            // Assert
            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            _studentRepoMock.Verify(repo => repo.UpdateStudent(studentId, mappedStudent), Times.Once);
        }

        [TestMethod]
        public void CalculateAge_ShouldReturnCorrectAge()
        {
            // Arrange
            var birthDate = new DateTime(2000, 1, 1);
            int expectedAge = DateTime.Today.Year - birthDate.Year;

            // Act
            int actualAge = _studentService.CalculateAge(birthDate);

            // Assert
            Assert.AreEqual(expectedAge, actualAge);
        }



    }
}
