using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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