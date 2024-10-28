using AutoMapper;
using SchoolApi.API.DTOs;
using SchoolApi.Business.Models;

namespace SchoolApi.API.Mapper
{
    public class StudentAutoMapperProfile:Profile
    {
        public StudentAutoMapperProfile()
        {

            CreateMap<StudentPostDto, Student>().ReverseMap();
            CreateMap<StudentRequestDto, Student>().ReverseMap();

        }
    }
}
