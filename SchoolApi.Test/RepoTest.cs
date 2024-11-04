using Bogus;
using Microsoft.EntityFrameworkCore;
using SchoolApi.Business.Data;
using SchoolApi.Business.Models;
using SchoolApi.Business.Repository;
using SchoolApi.Business.Services;

namespace SchoolApi.Test
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
            // Clear the in-memory database after each test
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
                .RuleFor(s => s.StudentGender, f => (Gender)f.Random.Int(1, 3))
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
            Assert.Equal(1, await _context.Students.CountAsync());
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
        public async Task DeleteStudent_ShouldMarkAsInactive()
        {
            // Arrange
            var student = GenerateFakeStudent();
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

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
            student1.FirstName = "Alice"; // Set specific name for search
            var student2 = GenerateFakeStudent();

            await _context.Students.AddRangeAsync(new[] { student1, student2 });
            await _context.SaveChangesAsync();

            // Act
            var result = await _studentRepo.GetSearchedStudents("Alice", 1, 10);

            // Assert
            Assert.Single(result.Data);
            Assert.Equal("Alice", result.Data.First().FirstName);
        }

        [Fact]
        public async Task GetStudentById_ShouldReturnCorrectStudent()
        {
            // Arrange
            var student = GenerateFakeStudent();
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            // Act
            var result = await _studentRepo.GetStudentById(student.StudentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(student.StudentId, result.StudentId);
        }

        //[Fact]
        //public async Task UpdateStudent_ShouldUpdateExistingStudent()
        //{
        //    // Arrange
        //    var student = GenerateFakeStudent();
        //    await _context.Students.AddAsync(student);
        //    await _context.SaveChangesAsync();

        //    var updatedData = new Student
        //    {
        //        FirstName = "UpdatedFirstName",
        //        LastName = "UpdatedLastName",
        //        StudentEmail = "updatedemail@example.com",

        //    };

        //    // Act
        //    await _studentRepo.UpdateStudent(updatedData);

        //    // Assert
        //    var updatedStudent = await _context.Students.FindAsync(student.StudentId);
        //    Assert.Equal("UpdatedFirstName", updatedStudent.FirstName);
        //    Assert.Equal("UpdatedLastName", updatedStudent.LastName);
        //    Assert.Equal("updatedemail@example.com", updatedStudent.StudentEmail);
        //}
    }
}