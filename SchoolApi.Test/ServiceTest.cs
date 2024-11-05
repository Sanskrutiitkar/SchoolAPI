using SchoolApi.Business.Services;

namespace SchoolApi.Test
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
            var expectedAge = 27 ;

            // Act
            var result = _studentService.CalculateAge(birthDate);

            // Assert
            Assert.NotEqual(expectedAge, result);
        }
    }
}


