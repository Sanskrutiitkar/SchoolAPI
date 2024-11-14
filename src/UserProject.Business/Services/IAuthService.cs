using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UserProject.Business.Models;

namespace UserProject.Business.Services
{
    public interface IAuthService
    {
         //Task<string> Login(string userEmail, string password);
         string GenerateToken(Claim[] claims);
         Task<Claim[]> GenerateClaims(Users user);
         public Task<Users> ValidateUser(string email, string password);
         public (string hashedPassword, string salt) HashPassword(string password);
         public bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt);
    }
}