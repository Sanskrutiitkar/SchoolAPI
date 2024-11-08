using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.Business.Services
{
    public interface IAuthService
    {
         Task<string> Login(string username, string password);
    }
}