using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.Api.DTOs
{
    public class UserRegistrationDto
    {
        public required string UserName { get; set; }
        public required string UserEmail { get; set; }
        public required string UserPassword { get; set; }
        public bool IsAdmin {get; set;}

    }
}