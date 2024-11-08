using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
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

        public async Task<string> Login(string username, string password)
        {
            var user = await _repo.ValidateUser(username, password);
            if (user == null || user == new Users())
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var claims = await GenerateClaims(user);
            var token = GenerateToken(claims);
            return token;
        }

        private static Task<Claim[]> GenerateClaims(Users user)
        {
            return Task.FromResult(new[]
            {
                new Claim("Name", user.UserName ?? throw new ArgumentNullException(nameof(user.UserName))),
                new Claim("Email", user.UserEmail ?? throw new ArgumentNullException(nameof(user.UserEmail))),
                new Claim(ClaimTypes.Role, user.IsAdmin ? "Admin" : "Teacher"),
                new Claim("Status", user.IsActive ? "Active" : "Inactive")
            });
        }

        private string GenerateToken(Claim[] claims)
        {
            var jwtKey = _configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key", "JWT key is not configured.");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        } 
    }
}