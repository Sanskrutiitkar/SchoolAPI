using API_School.Models;
using Microsoft.EntityFrameworkCore;

namespace API_School.Data
{
    public class StudentDbContext: DbContext
    {
        public DbSet<Student> Students { get; set; }
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }
    }
}
