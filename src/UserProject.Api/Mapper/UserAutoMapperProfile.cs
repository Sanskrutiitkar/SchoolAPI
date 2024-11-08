using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using UserProject.Api.DTOs;
using UserProject.Business.Models;

namespace UserProject.Api.Mapper
{
    public class UserAutoMapperProfile:Profile
    {
         public UserAutoMapperProfile()
        {
            CreateMap<LoginRequestDto, Users>().ReverseMap();
            CreateMap<UserRegistrationDto, Users>().ReverseMap();

        }
    }
}