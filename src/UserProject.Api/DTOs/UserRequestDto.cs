using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserProject.Api.DTOs
{
    
    public class UserRequestDto
    {
        public string UserName { get; set; }= string.Empty;
        public string UserEmail { get; set; }= string.Empty;
        public bool IsAdmin {get; set;}

    }
   
}