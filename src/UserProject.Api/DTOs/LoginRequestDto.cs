using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.Api.DTOs
{
    public class LoginRequestDto
    {
        public string UserEmail { get; set; }
        public string Password { get; set; }
    }
}