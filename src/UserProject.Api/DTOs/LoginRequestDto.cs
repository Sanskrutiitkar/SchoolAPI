using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.Api.DTOs
{
    public class LoginRequestDto
    {       
        public string UserEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}