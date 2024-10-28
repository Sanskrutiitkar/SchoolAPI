
using Microsoft.EntityFrameworkCore;
using SchoolApi.Business.Data;
using SchoolApi.Business.Exceptions;
using SchoolApi.Business.Models;


namespace SchoolApi.Business.Repository
{
    public class StudentRepo : IStudentRepo
    {
        private readonly StudentDbContext _context;

        public StudentRepo(StudentDbContext context)
        {
            _context = context;
        }

        public async Task<Student> AddStudent(Student student)
        {
            if (student == null) 
            {
                return null;
            }
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task DeleteStudent(int studentID)
        {
            var student = await _context.Students.FindAsync(studentID);
            if (student == null)
            {
                throw new StudentNotFoundException("Student with this ID not found");
            }

            if (!student.IsActive)
            {
                throw new InvalidOperationException("Student is already inactive.");
            }
            student.IsActive = false;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Student>> GetAllStudents()
        {
            return await _context.Students.ToListAsync() ?? new List<Student>();
        }

        public async Task UpdateStudent(int id, Student student)
        {
            var requiredStudent = await _context.Students.FindAsync(id);
            if (requiredStudent == null)
            {
                throw new StudentNotFoundException("Student with this ID does not exist");
            }

            requiredStudent.FirstName = student.FirstName;
            requiredStudent.LastName = student.LastName;
            requiredStudent.StudentPhone = student.StudentPhone;
            requiredStudent.StudentEmail = student.StudentEmail;
            requiredStudent.BirthDate = student.BirthDate;
            requiredStudent.StudentGender = student.StudentGender;

            await _context.SaveChangesAsync();
        }

        public async Task<PagedResponse<Student>> GetSearchedStudents(string search, int pageNumber, int pageSize)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.FirstName.Contains(search) || p.LastName.Contains(search) || p.StudentEmail.Contains(search) || p.StudentAge.ToString() == search);
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResponse<Student>(items, pageNumber, pageSize, totalCount);
        }
    }
}
