using API_School.Data;
using API_School.DTO;
using API_School.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_School.Repository
{
    public class StudentRepo:IStudentRepo
    {
        private readonly StudentDbContext _context;
        private readonly IMapper _mapper;
        public StudentRepo(StudentDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IActionResult> AddStudent(Student student)
        {
  
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();
            return new OkObjectResult(new { message = "Student Added Successfully" });

        }

        public async Task<IActionResult> DeleteStudent(int StudentID)
        {
            Student student = await _context.Students.FindAsync(StudentID);
            if (student == null)
            {
                return new BadRequestObjectResult(new { message = "Student with this id not found" });
            }
            else
            {
                student.IsActive = false;
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { message = "Student deleted succesfully" });
            }


        }

        public async Task<IEnumerable<Student>> GetAllStudents()
        {
            return await _context.Students.ToListAsync() ?? new List<Student>();
        }

        public async Task<IActionResult> UpdateStudent(int id, Student student)
        {
            Student Requiredstudent = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return new BadRequestObjectResult(new { message = "Student With this id does not exists" });
            }
            else
            {
                // Requiredstudent = student --> not working
                Requiredstudent.FirstName = student.FirstName;
                Requiredstudent.LastName = student.LastName;
                Requiredstudent.StudentPhone = student.StudentPhone;
                Requiredstudent.StudentEmail = student.StudentEmail;
                Requiredstudent.BirthDate = student.BirthDate;
                Requiredstudent.StudentGender = student.StudentGender;
                await _context.SaveChangesAsync();
                return new OkObjectResult(new { message = $"Student with id {id} updates succesfully" });
            }

        }
        public async Task<ActionResult<PagedResponse<StudentRequestDto>>> GetSearchedStudents(string search, int pageNumber, int pageSize)
        {
            var query = _context.Students.AsQueryable();
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.FirstName.Contains(search) || p.LastName.Contains(search) || p.StudentEmail.Contains(search) || p.StudentAge.ToString() == search);
            }
            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            var dtoResponse = _mapper.Map<IEnumerable<StudentRequestDto>>(items);

            var pagedResponse = new PagedResponse<StudentRequestDto>(dtoResponse, pageNumber, pageSize, totalCount);
            return pagedResponse;
        }

    }
}
