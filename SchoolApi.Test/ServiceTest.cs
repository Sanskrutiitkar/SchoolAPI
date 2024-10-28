using Moq;
using SchoolApi.Business.Models;
using SchoolApi.Business.Repository;
using SchoolApi.Business.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolApi.Test
{
    public class ServiceTest
    {
        private readonly Mock<IStudentRepo> _studentRepoMock;
        private readonly StudentService _studentService;

        public ServiceTest()
        {
            _studentRepoMock = new Mock<IStudentRepo>();
            _studentService = new StudentService(_studentRepoMock.Object);
        }

        [Fact]
        public async Task AddStudent_ShouldCallRepoAddStudent()
        {
            // Arrange
            var student = new Student
            {
                FirstName = "Sanskruti",
                LastName = "Itkar",
                StudentEmail = "sanskruti.itkar@example.com",
                StudentPhone = "1234567890",
                BirthDate = new DateTime(2000, 1, 1),
                StudentAge = 24,
                StudentGender = Gender.MALE
            };

            _studentRepoMock.Setup(repo => repo.AddStudent(student)).ReturnsAsync(student);

            // Act
            var result = await _studentService.AddStudent(student);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(student.FirstName, result.FirstName);
            _studentRepoMock.Verify(repo => repo.AddStudent(student), Times.Once);
        }

        [Fact]
        public async Task DeleteStudent_ShouldCallRepoDeleteStudent()
        {
            // Arrange
            int studentId = 1;

            // Act
            await _studentService.DeleteStudent(studentId);

            // Assert
            _studentRepoMock.Verify(repo => repo.DeleteStudent(studentId), Times.Once);
        }

        [Fact]
        public async Task GetAllStudents_ShouldCallRepoGetAllStudents()
        {
            // Arrange
            var students = new List<Student>
            {
                new Student { FirstName = "Alice", LastName = "Smith" },
                new Student { FirstName = "Bob", LastName = "Brown" }
            };

            _studentRepoMock.Setup(repo => repo.GetAllStudents()).ReturnsAsync(students);

            // Act
            var result = await _studentService.GetAllStudents();

            // Assert
            Assert.Equal(2, result.Count());
            _studentRepoMock.Verify(repo => repo.GetAllStudents(), Times.Once);
        }

        [Fact]
        public async Task GetSearchedStudents_ShouldCallRepoGetSearchedStudents()
        {
            // Arrange
            string searchTerm = "sanskruti";
            int pageNumber = 1;
            int pageSize = 10;
            var pagedResponse = new PagedResponse<Student>(new List<Student>(), pageNumber, pageSize, 0);

            _studentRepoMock.Setup(repo => repo.GetSearchedStudents(searchTerm, pageNumber, pageSize)).ReturnsAsync(pagedResponse);

            // Act
            var result = await _studentService.GetSearchedStudents(searchTerm, pageNumber, pageSize);

            // Assert
            Assert.Equal(pagedResponse, result);
            _studentRepoMock.Verify(repo => repo.GetSearchedStudents(searchTerm, pageNumber, pageSize), Times.Once);
        }

        [Fact]
        public async Task UpdateStudent_ShouldCallRepoUpdateStudent()
        {
            // Arrange
            int studentId = 1;
            var student = new Student { FirstName = "Charlie" };

            // Act
            await _studentService.UpdateStudent(studentId, student);

            // Assert
            _studentRepoMock.Verify(repo => repo.UpdateStudent(studentId, student), Times.Once);
        }

        [Fact]
        public void CalculateAge_ShouldReturnCorrectAge()
        {
            // Arrange
            var birthDate = new DateTime(2000, 1, 1);
            var expectedAge = DateTime.Today.Year - birthDate.Year;

            // Act
            var result = _studentService.CalculateAge(birthDate);

            // Assert
            Assert.Equal(expectedAge, result);
        }
    }
}


