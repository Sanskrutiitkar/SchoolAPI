using API_School.DTO;
using API_School.Models;
using AutoMapper;

namespace API_School.Helper
{
    public class StudentAutoMapperProfile : Profile
    {
        public StudentAutoMapperProfile()
        {
            
            CreateMap<Student, StudentPostDto>();
            CreateMap<StudentPostDto, Student>();
            CreateMap<StudentRequestDto, Student>().ReverseMap();

        }

    }
}
