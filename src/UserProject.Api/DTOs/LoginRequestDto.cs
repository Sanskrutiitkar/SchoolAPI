using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.Api.DTOs
{
    public class LoginRequestDto
    {       
        public required string UserEmail { get; set; } 
        public required string Password { get; set; } 
    }
}