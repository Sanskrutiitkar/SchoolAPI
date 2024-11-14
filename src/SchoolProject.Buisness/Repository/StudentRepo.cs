
using Microsoft.EntityFrameworkCore;
using SchoolProject.Buisness.Data;
using SchoolProject.Buisness.Models;

namespace SchoolProject.Buisness.Repository
{
    public class StudentRepo:IStudentRepo
    {
        private readonly StudentDbContext _context;

        public StudentRepo(StudentDbContext context)
        {
            _context = context;
        }
        public async Task<Student> AddStudent(Student student)
        {
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task DeleteStudent(int studentID)
        {
            var student = await _context.Students.FindAsync(studentID);         
            student.IsActive = false; 
            await _context.SaveChangesAsync();
                     
        }

        public async Task<IEnumerable<Student>> GetAllStudents()
        {
            return await _context.Students.Where(s=>s.IsActive==true).ToListAsync();
        }

        public async Task<Student?> GetStudentById(int id)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.StudentId == id && s.IsActive);            
        }

        public async Task<Student> UpdateStudent(Student student)
        {                    
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<PagedResponse<Student>> GetSearchedStudents(string search, int pageNumber, int pageSize)
        {
            var query = _context.Students.AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.FirstName.Contains(search) ||
                                         p.LastName.Contains(search) ||
                                         p.StudentEmail.Contains(search) ||
                                         p.StudentAge.ToString() == search);
            }

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PagedResponse<Student>(items, pageNumber, pageSize, totalCount);
        }

        public async Task<Student?> CheckDuplicate(Student student)
        {
            return await _context.Students.FirstOrDefaultAsync(s => s.StudentEmail == student.StudentEmail && s.IsActive);           
        }
    }
}