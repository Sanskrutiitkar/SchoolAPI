
using AutoMapper;
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
            CreateMap<UserRequestDto,Users>().ReverseMap();

        }
    }
}