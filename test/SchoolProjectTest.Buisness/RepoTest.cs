using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using Microsoft.EntityFrameworkCore;
using SchoolProject.Buisness.Data;
using SchoolProject.Buisness.Models;
using SchoolProject.Buisness.Repository;
using Xunit;

namespace SchoolProjectTest.Buisness
{
    public class RepoTest: IDisposable
    {

        private readonly StudentDbContext _context;
        private readonly StudentRepo _studentRepo;

        public RepoTest()
        {
            var options = new DbContextOptionsBuilder<StudentDbContext>()
      .UseInMemoryDatabase("TestDB") 
      .Options;

            _context = new StudentDbContext(options);
            _studentRepo = new StudentRepo(_context);
        }
        public void Dispose()
        {         
            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();
        }
        private Student GenerateFakeStudent()
        {
            var faker = new Faker<Student>()
                .RuleFor(s => s.FirstName, f => f.Name.FirstName())
                .RuleFor(s => s.LastName, f => f.Name.LastName())
                .RuleFor(s => s.StudentEmail, f => f.Internet.Email())
                .RuleFor(s => s.StudentPhone, f => f.Phone.PhoneNumber())
                .RuleFor(s => s.BirthDate, f => f.Date.Past(20))
                .RuleFor(s => s.StudentAge, f => DateTime.Now.Year - f.Date.Past(20).Year)
                .RuleFor(s => s.StudentGender, f => f.PickRandom<Gender>())
                .RuleFor(s => s.IsActive, true); 

            return faker.Generate();
        }

        [Fact]
        public async Task AddStudent_ShouldAddStudent()
        {
            // Arrange
            var student = GenerateFakeStudent();

            // Act
            var result = await _studentRepo.AddStudent(student);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(student.FirstName, result.FirstName);
            Assert.Equal(student.LastName, result.LastName);
            Assert.Equal(student.StudentEmail, result.StudentEmail);
            Assert.Equal(student.StudentPhone, result.StudentPhone);
            Assert.Equal(student.BirthDate, result.BirthDate);
            Assert.Equal(student.StudentAge, result.StudentAge);
            Assert.Equal(student.StudentGender, result.StudentGender);
            Assert.Equal(student.IsActive, result.IsActive);
            Assert.Equal(1, await _context.Students.CountAsync());
        }
        [Fact]
        public async Task Add_ShouldThrowException_WhenStudentIsNull()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => _studentRepo.AddStudent(null));
        }

        [Fact]
        public async Task GetAllStudents_ShouldReturnAllStudents()
        {
            // Arrange
            var students = new List<Student>
        {
            GenerateFakeStudent(),
            GenerateFakeStudent()
        };

            await _context.Students.AddRangeAsync(students);
            await _context.SaveChangesAsync();

            // Act
            var result = await _studentRepo.GetAllStudents();

            // Assert
            Assert.Equal(2, result.Count());
              
        }
        [Fact]
        public async Task GetAll_ShouldReturnEmpty_WhenNoActiveStudents_WorstCase()
        {
            var student1 = GenerateFakeStudent();
            student1.IsActive = false;
            await _studentRepo.AddStudent(student1);

            var result = await _studentRepo.GetAllStudents();

            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteStudent_ShouldMarkAsInactive()
        {
            // Arrange
            var student = GenerateFakeStudent();
            await _studentRepo.AddStudent(student);
           

            // Act
            await _studentRepo.DeleteStudent(student.StudentId);

            // Assert
            var deletedStudent = await _context.Students.FindAsync(student.StudentId);
            Assert.False(deletedStudent.IsActive);
        }


        [Fact]
        public async Task GetSearchedStudents_ShouldReturnFilteredResults()
        {
            // Arrange
            var student1 = GenerateFakeStudent();
            student1.FirstName = "sanskruti"; 
            var student2 = GenerateFakeStudent();

            await _context.Students.AddRangeAsync(new[] { student1, student2 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _studentRepo.GetSearchedStudents("sanskruti", 1, 10);

            // Assert
            Assert.Single(result.Data);
            Assert.Equal("sanskruti", result.Data.First().FirstName);
        }
        [Fact]
        public async Task FilterStudents_ShouldReturnEmpty_WhenNoMatches_WorstCase()
        {
            var student1 = GenerateFakeStudent();
            await _studentRepo.AddStudent(student1);

            var result = await _studentRepo.GetSearchedStudents("NonExistent",1, 10);

            Assert.Empty(result.Data);
            Assert.Equal(0, result.TotalRecords);
        }
        [Fact]
        public async Task GetStudentById_ShouldReturnCorrectStudent()
        {
            // Arrange
            var student = GenerateFakeStudent();
            await _studentRepo.AddStudent(student);

            // Act
            var result = await _studentRepo.GetStudentById(student.StudentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(student.StudentId, result.StudentId);
            Assert.Equal(student.FirstName, result.FirstName);
            Assert.Equal(student.LastName, result.LastName);
            Assert.Equal(student.StudentEmail, result.StudentEmail);
            Assert.Equal(student.StudentPhone, result.StudentPhone);
            Assert.Equal(student.BirthDate, result.BirthDate);
            Assert.Equal(student.StudentAge, result.StudentAge);
            Assert.Equal(student.StudentGender, result.StudentGender);
            Assert.Equal(student.IsActive, result.IsActive);
        }
        [Fact]
        public async Task GetById_ShouldReturnNull_WhenNotExists_WorstCase()
        {
            var result = await _studentRepo.GetStudentById(999);
            Assert.Null(result);
        }
        [Fact]
        public async Task UpdateStudent_ShouldUpdateExistingStudent()
        {
            // Arrange
            var student = GenerateFakeStudent();
            await _studentRepo.AddStudent(student);          
            student.FirstName = "sanskruti";

            // Act
            await _studentRepo.UpdateStudent(student);

            // Assert
            var updatedStudent = await _context.Students.FindAsync(student.StudentId);
            Assert.Equal("sanskruti", updatedStudent.FirstName);

        }
    
       
    }
}