using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserProject.Business.Data;
using UserProject.Business.Models;

namespace UserProject.Business.Repository
{
    public class AuthRepo:IAuthRepo
    {
        private readonly UserDbContext _context;
        public AuthRepo(UserDbContext context)
        {
            _context = context;
        }
        public async Task<Users> ValidateUser(string email, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(i => i.UserEmail == email && i.UserPassword == password) ?? new Users();
        }
    }
}