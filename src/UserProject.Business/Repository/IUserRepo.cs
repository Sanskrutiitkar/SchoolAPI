using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserProject.Business.Models;

namespace UserProject.Business.Repository
{
    public interface IUserRepo
    {
        Task<Users> GetUserByEmail(string userEmail);
        Task<Users?> GetUserById(int userId);
        Task<Users> AddUser(Users user);
        Task DeleteUser(int userId);
        Task<IEnumerable<Users>> GetAllUser();
    }
}