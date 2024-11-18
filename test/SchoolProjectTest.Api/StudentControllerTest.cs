using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using SchoolProject.Api.Controller;
using SchoolProject.Api.Mapper;
using SchoolProject.Buisness.Repository;
using SchoolProject.Buisness.Services;

namespace SchoolProjectTest.Api
{
    public class StudentControllerTest
    {
        private readonly Mock<IStudentRepo> _mockStudentRepo;
        private readonly Mock<IStudentService> _mockStudentService;
        private readonly IMapper _mapper;
        private readonly StudentController _controller;

        public StudentControllerTest()
        {
            // Configure AutoMapper using the StudentAutoMapperProfile
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new StudentAutoMapperProfile());  // Your profile class
            });
            _mapper = mappingConfig.CreateMapper();

            // Mock the repository and service
            _mockStudentRepo = new Mock<IStudentRepo>();
            _mockStudentService = new Mock<IStudentService>();
            _controller = new StudentController(_mockStudentService.Object, _mapper, _mockStudentRepo.Object);
        }
    }
}