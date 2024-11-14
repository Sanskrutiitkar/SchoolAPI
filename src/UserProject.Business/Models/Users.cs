using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UserProject.Business.Models
{
    public class Users
    {
        [Key]
        public int UserId {get; set;}
        public  string UserEmail {get; set;}
        public  string UserName {get; set;}
        public  string UserPassword {get; set;}
        public string PasswordSalt{get; set;}
        public  bool IsAdmin {get; set;}
        public  bool IsActive {get; set;} = true;
    }
}