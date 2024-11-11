using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.Api.DTOs
{
    public class UserRequestDto
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public bool IsAdmin {get; set;}

    }
}