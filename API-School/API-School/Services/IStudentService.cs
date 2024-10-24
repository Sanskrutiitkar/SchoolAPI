using API_School.DTO;
using API_School.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_School.Services
{
    public interface IStudentService
    {
        Task<ActionResult<PagedResponse<StudentRequestDto>>> GetSearchedStudents(string search, int pageNumber, int pageSize);
        Task<IEnumerable<StudentRequestDto>> GetAllStudents();
        Task<IActionResult> AddStudent(StudentPostDto student);
        Task<IActionResult> UpdateStudent(int id, StudentPostDto student);
        Task<IActionResult> DeleteStudent(int StudentID);
    }
}
