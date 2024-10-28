using Microsoft.EntityFrameworkCore;
using SchoolApi.Business.Data;
using SchoolApi.Business.Exceptions;
using SchoolApi.Business.Models;
using SchoolApi.Business.Repository;

namespace SchoolApi.Test
{
    
    public class RepoTest
    {
        private readonly DbContextOptions<StudentDbContext> _options;

        public RepoTest()
        {
            _options = new DbContextOptionsBuilder<StudentDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
                .Options;
        }

        private StudentDbContext CreateContext()
        {
            return new StudentDbContext(_options);
        }

        [Fact]
        public async Task AddStudent_ShouldAddStudent()
        {
            using var context = CreateContext();
            var repo = new StudentRepo(context);

            var student = new Student
            {
                FirstName = "Sanskruti",
                LastName = "Itkar",
                StudentEmail = "sans.itkar@example.com",
                StudentPhone = "1234567890",
                BirthDate = new DateTime(2000, 1, 1),
                StudentAge = 24,
                StudentGender = Gender.FEMALE
            };

            var result = await repo.AddStudent(student);

            Assert.NotNull(result);
            Assert.Equal("Sanskruti", result.FirstName);
            Assert.Equal(1, await context.Students.CountAsync());
        }

        [Fact]
        public async Task GetAllStudents_ShouldReturnAllStudents()
        {
            using var context = CreateContext();
            var repo = new StudentRepo(context);

            await repo.AddStudent(new Student
            {
                FirstName = "Srushti",
                LastName = "Johnson",
                StudentEmail = "srushti.johnson@example.com",
                StudentPhone = "1231231234",
                BirthDate = new DateTime(2002, 1, 1),
                StudentAge = 22,
                StudentGender = Gender.OTHER
            });
            await repo.AddStudent(new Student
            {
                FirstName = "Sita",
                LastName = "Joshi",
                StudentEmail = "sita.joshi@example.com",
                StudentPhone = "0987654321",
                BirthDate = new DateTime(2001, 1, 1),
                StudentAge = 23,
                StudentGender = Gender.FEMALE
            });

            var students = await repo.GetAllStudents();

            Assert.Equal(2, students.Count());
        }

        [Fact]
        public async Task UpdateStudent_ShouldUpdateStudent()
        {
            using var context = CreateContext();
            var repo = new StudentRepo(context);

            var student = new Student
            {
                FirstName = "ram",
                LastName = "sham",
                StudentEmail = "ram22@example.com",
                StudentPhone = "1231231234",
                BirthDate = new DateTime(2002, 1, 1),
                StudentAge = 22,
                StudentGender = Gender.MALE
            };

            await repo.AddStudent(student);

            // Update the student's name
            student.FirstName = "ramprakash";
            await repo.UpdateStudent(student.StudentId, student);

            var updatedStudent = await context.Students.FindAsync(student.StudentId);
            Assert.Equal("ramprakash", updatedStudent.FirstName);
        }

        [Fact]
        public async Task GetSearchedStudents_ShouldReturnFilteredResults()
        {
            using var context = CreateContext();
            var repo = new StudentRepo(context);

            await repo.AddStudent(new Student
            {
                FirstName = "Diana",
                LastName = "Prince",
                StudentEmail = "diana.prince@example.com",
                StudentPhone = "5555555555",
                BirthDate = new DateTime(1990, 1, 1),
                StudentAge = 34,
                StudentGender = Gender.FEMALE
            });
            await repo.AddStudent(new Student
            {
                FirstName = "Bruce",
                LastName = "Wayne",
                StudentEmail = "bruce.wayne@example.com",
                StudentPhone = "6666666666",
                BirthDate = new DateTime(1985, 5, 19),
                StudentAge = 39,
                StudentGender = Gender.MALE
            });
            await repo.AddStudent(new Student
            {
                FirstName = "Clark",
                LastName = "Kent",
                StudentEmail = "clark.kent@example.com",
                StudentPhone = "7777777777",
                BirthDate = new DateTime(1980, 6, 18),
                StudentAge = 44,
                StudentGender = Gender.MALE
            });

            var result = await repo.GetSearchedStudents("Bruce", 1, 10);

            Assert.Single(result.Data);
            Assert.Equal("Bruce", result.Data.First().FirstName);
        }

        [Fact]
        public async Task DeleteStudent_ShouldMarkStudentAsInactive()
        {
            using var context = CreateContext();
            var repo = new StudentRepo(context);

            var student = new Student
            {
                FirstName = "Alice",
                LastName = "Smith",
                StudentEmail = "alice.smith@example.com",
                StudentPhone = "1231231234",
                BirthDate = new DateTime(2000, 1, 1),
                StudentAge = 24,
                StudentGender = Gender.FEMALE
            };

            await repo.AddStudent(student);
            await repo.DeleteStudent(student.StudentId);

            var deletedStudent = await context.Students.FindAsync(student.StudentId);
            Assert.False(deletedStudent.IsActive);
        }

        [Fact]
        public async Task DeleteStudent_ShouldThrowException_WhenStudentNotFound()
        {
            using var context = CreateContext();
            var repo = new StudentRepo(context);

            await Assert.ThrowsAsync<StudentNotFoundException>(async () =>
            {
                await repo.DeleteStudent(999); 
            });
        }
        [Fact]
        public async Task DeleteStudent_ShouldThrowException_WhenStudentIsAlreadyInactive()
        {
            using var context = CreateContext();
            var repo = new StudentRepo(context);

            // Arrange
            var student = new Student
            {
                FirstName = "Alice",
                LastName = "Smith",
                StudentEmail = "alice.smith@example.com",
                StudentPhone = "1231231234",
                BirthDate = new DateTime(2000, 1, 1),
                StudentAge = 24,
                StudentGender = Gender.FEMALE,
                IsActive = false 
            };

            await repo.AddStudent(student);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            {
                await repo.DeleteStudent(student.StudentId); 
            });
        }

        [Fact]
        public async Task AddStudent_ShouldFail_WhenStudentIsNull()
        {
            using var context = CreateContext();
            var repo = new StudentRepo(context);

            // Act
            var result = await repo.AddStudent(null);

            // Assert
            Assert.Null(result); 
            Assert.Empty(context.Students); 
        }

        [Fact]
        public async Task GetAllStudents_ShouldFail_WhenNoStudentsAdded()
        {
            using var context = CreateContext();
            var repo = new StudentRepo(context);

            // Act
            var students = await repo.GetAllStudents();

            // Assert
            Assert.Empty(students); 
        }

        [Fact]
        public async Task UpdateStudent_ShouldThrowException_WhenStudentNotFound()
        {
            using var context = CreateContext();
            var repo = new StudentRepo(context);

            var student = new Student
            {
                FirstName = "ram",
                LastName = "sham",
                StudentEmail = "ram22@example.com",
                StudentPhone = "1231231234",
                BirthDate = new DateTime(2002, 1, 1),
                StudentAge = 22,
                StudentGender = Gender.MALE
            };

            // Act & Assert
            await Assert.ThrowsAsync<StudentNotFoundException>(async () =>
            {
                await repo.UpdateStudent(999, student); 
            });
        }

        [Fact]
        public async Task GetSearchedStudents_ShouldReturnEmpty_WhenNoMatchesFound()
        {
            using var context = CreateContext();
            var repo = new StudentRepo(context);

            // Adding a student
            await repo.AddStudent(new Student
            {
                FirstName = "Diana",
                LastName = "Prince",
                StudentEmail = "diana.prince@example.com",
                StudentPhone = "5555555555",
                BirthDate = new DateTime(1990, 1, 1),
                StudentAge = 34,
                StudentGender = Gender.FEMALE
            });

            // Act
            var result = await repo.GetSearchedStudents("Bruce", 1, 10); 

            // Assert
            Assert.Empty(result.Data); 
        }

 
    }
}