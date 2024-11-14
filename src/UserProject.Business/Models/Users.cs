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
        public  string UserEmail {get; set;}= string.Empty;
        public  string UserName {get; set;}= string.Empty;
        public  string UserPassword {get; set;}= string.Empty;
        public string PasswordSalt{get; set;}= string.Empty;
        public  bool IsAdmin {get; set;}
        public  bool IsActive {get; set;} = true;
    }
}