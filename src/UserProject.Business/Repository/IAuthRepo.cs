using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserProject.Business.Models;

namespace UserProject.Business.Repository
{
    public interface IAuthRepo
    {
        Task<Users?> ValidateUser(string userEmail);
    }
}