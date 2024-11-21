using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using UserProject.Business.Models;
using UserProject.Business.Repository;

namespace UserProject.Business.Services
{
    public class AuthService:IAuthService
    {
        private readonly IAuthRepo _repo;
        private readonly IConfiguration _configuration;

        public AuthService(IAuthRepo repo, IConfiguration configuration)
        {
            _repo = repo;
            _configuration = configuration;
        }

        public async Task<Users> ValidateUser(string email, string password)
        {
            
            var user = await _repo.ValidateUser(email);

            if (user == null)
            {
                return null; 
            }

              if (VerifyPassword(password, user.UserPassword, user.PasswordSalt))
            {
                return user;
            }

            return null; 
        }


        public  Task<Claim[]> GenerateClaims(Users user)
        {
            return Task.FromResult(new[]
            {
                new Claim("Name", user.UserName ?? throw new ArgumentNullException(nameof(user.UserName))),
                new Claim("Email", user.UserEmail ?? throw new ArgumentNullException(nameof(user.UserEmail))),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Teacher"),
            });
        }

        public string GenerateToken(Claim[] claims)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT key is not configured.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(10),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        } 
        public (string hashedPassword, string salt) HashPassword(string password)
        {
        // Generate a unique salt
        using (var rng = new RNGCryptoServiceProvider())
        {
            byte[] saltBytes = new byte[16]; // 128-bit salt
            rng.GetBytes(saltBytes);
            string salt = Convert.ToBase64String(saltBytes);

            // Use PBKDF2 to hash the password with the salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, 10000)) // 10000 iterations
            {
                byte[] hashBytes = pbkdf2.GetBytes(32); // 256-bit hash
                string hash = Convert.ToBase64String(hashBytes);

                return (hash, salt); // Return the hash and the salt as base64 strings
            }
        }
        }
        public bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            // Convert the stored salt from base64 back to bytes
            byte[] saltBytes = Convert.FromBase64String(storedSalt);

            // Use PBKDF2 to hash the entered password with the stored salt
            using (var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, saltBytes, 10000))
            {
                byte[] enteredHashBytes = pbkdf2.GetBytes(32); // 256-bit hash
                string enteredHash = Convert.ToBase64String(enteredHashBytes);

                // Compare the hashes
                return enteredHash == storedHash;
            }
        }

    }

    }

