
using Microsoft.EntityFrameworkCore;
using UserProject.Business.Models;

namespace UserProject.Business.Data
{
    public class UserDbContext: DbContext
    {
        public DbSet<Users> Users { get; set; }
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
 
    }    
}