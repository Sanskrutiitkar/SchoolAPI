
using AutoMapper;
using SchoolProject.Api.DTOs;
using SchoolProject.Buisness.Models;

namespace SchoolProject.Api.Mapper
{
    public class StudentAutoMapperProfile:Profile
    {
         public StudentAutoMapperProfile()
        {
            CreateMap<StudentPostDto, Student>().ReverseMap();
            CreateMap<StudentRequestDto, Student>().ReverseMap();
            CreateMap<UpdateStudentDto, Student>().ReverseMap();
        }

    }
}