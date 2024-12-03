
using SchoolProject.Buisness.Models;

namespace SchoolProject.Buisness.Repository
{
    public interface ICourseRepo
    {
        Task<Course> GetCourseById(int courseId);
        Task<Course> AddCourse(Course course); 
    }
}