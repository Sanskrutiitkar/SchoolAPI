using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SchoolProject.Buisness.Services;
using Xunit;

namespace SchoolProjectTest.Buisness
{
    public class ServiceTest
    {
        private readonly StudentService _studentService;

        public ServiceTest()
        {
            _studentService = new StudentService();
        }


        [Fact]
        public void CalculateAge_ShouldReturnCorrectAge()
        {
            // Arrange
            var birthDate = new DateTime(2000, 1, 1);
            var expectedAge = DateTime.Today.Year - birthDate.Year;

            // Act
            var result = _studentService.CalculateAge(birthDate);

            // Assert
            Assert.Equal(expectedAge, result);
        }
        [Fact]
        public void CalculateAge_ShouldReturnIncorrectAge()
        {
            // Arrange
            var birthDate = new DateTime(2000, 1, 1);
            var expectedAge = DateTime.Today.Year - birthDate.Year + 5 ;

            // Act
            var result = _studentService.CalculateAge(birthDate);

            // Assert
            Assert.NotEqual(expectedAge, result);
        }
    }
}