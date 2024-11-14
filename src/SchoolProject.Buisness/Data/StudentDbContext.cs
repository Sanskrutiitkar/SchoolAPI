
using Microsoft.EntityFrameworkCore;
using SchoolProject.Buisness.Models;

namespace SchoolProject.Buisness.Data
{
    public class StudentDbContext:DbContext
    {
         public DbSet<Student> Students { get; set; }
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }
 
    }
}