
using Microsoft.EntityFrameworkCore;
using UserProject.Business.Data;
using UserProject.Business.Models;

namespace UserProject.Business.Repository
{
    public class UserRepo : IUserRepo
    {
        private readonly UserDbContext _context;
        public UserRepo(UserDbContext context)
        {
            _context = context;
        }
        public async Task<Users> AddUser(Users user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task DeleteUser(int userId)
        {
            var user = await _context.Users.FindAsync(userId);         
            user.IsActive = false; 
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Users>> GetAllUser()
        {
            return await _context.Users.Where(s=>s.IsActive==true).ToListAsync();
        }

        public async Task<Users?> GetUserById(int userId)
        {
             return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);;
     
        }
        public async Task<Users?> GetUserByEmail(string userEmail)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.UserEmail == userEmail && u.IsActive);        
        }
    }
}