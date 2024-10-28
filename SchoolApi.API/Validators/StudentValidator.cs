using FluentValidation;
using SchoolApi.API.DTOs;
using SchoolApi.Business.Models;

namespace SchoolApi.API.Validators
{
    public class StudentValidator:AbstractValidator<StudentPostDto>
    {
        public StudentValidator()
    {
        RuleFor(x => x.FirstName).MinimumLength(2).MaximumLength(10).WithMessage("Please specify a valid first name").NotEmpty();
        RuleFor(x => x.LastName).MinimumLength(2).MaximumLength(10).WithMessage("Please specify a valid last name").NotEmpty();
        RuleFor(x => x.StudentPhone).Length(10).WithMessage("The Phone Number should contain 10 digits");
        RuleFor(x => x.StudentEmail).EmailAddress().WithMessage("please provide valid email");
        RuleFor(x => x.BirthDate).Must(BeAValidBirthDate).WithMessage("Please specify a valid birthdate");
        RuleFor(x => x.StudentGender).Must(BeAValidGender).WithMessage("provide proper gender as MALE/FEMALE/OTHER : (1/2/3)").NotEmpty();


    }
    private bool BeAValidGender(Gender gender)
    {
        if (gender.Equals(Gender.FEMALE)) return true;

        else if (gender.Equals(Gender.MALE)) return true;

        else if (gender.Equals(Gender.OTHER)) return true;

        return false;

    }

    private bool BeAValidBirthDate(DateTime BirthDate)
    {
        var minDate = DateTime.Now.AddYears(-10);
        return BirthDate <= minDate;

    }


}
      
}
