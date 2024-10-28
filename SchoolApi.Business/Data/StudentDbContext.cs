using Microsoft.EntityFrameworkCore;
using SchoolApi.Business.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolApi.Business.Data
{
    public class StudentDbContext:DbContext
    {
        public DbSet<Student> Students { get; set; }
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }
    }
}
