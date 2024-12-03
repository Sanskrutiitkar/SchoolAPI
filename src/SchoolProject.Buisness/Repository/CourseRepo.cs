
using Microsoft.EntityFrameworkCore;
using SchoolProject.Buisness.Data;
using SchoolProject.Buisness.Models;

namespace SchoolProject.Buisness.Repository
{
    public class CourseRepo:ICourseRepo
    {
        private readonly StudentDbContext _context;

        public CourseRepo(StudentDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<Course> GetCourseById(int courseId)
        {
            return await _context.Courses.FirstOrDefaultAsync(c => c.CourseId == courseId);
        }

        public async Task<Course> AddCourse(Course course)
        {
            await _context.Courses.AddAsync(course);
            await _context.SaveChangesAsync();
            return course;
        }
    }
}