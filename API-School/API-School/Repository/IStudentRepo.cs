using API_School.DTO;
using API_School.Models;
using Microsoft.AspNetCore.Mvc;

namespace API_School.Repository
{
    public interface IStudentRepo
    {
        Task<ActionResult<PagedResponse<StudentRequestDto>>> GetSearchedStudents(string search, int pageNumber, int pageSize);
        Task<IEnumerable<Student>> GetAllStudents();
        Task<IActionResult> AddStudent(Student student);
        Task<IActionResult> UpdateStudent(int id, Student student);
        Task<IActionResult> DeleteStudent(int StudentID);
    }
}
