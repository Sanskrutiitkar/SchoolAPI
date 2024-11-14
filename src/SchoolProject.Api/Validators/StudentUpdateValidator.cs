
using FluentValidation;
using SchoolProject.Api.DTOs;
using SchoolProject.Buisness.Models;

namespace SchoolProject.Api.Validators
{
     public class StudentUpdateValidator : AbstractValidator<UpdateStudentDto>
    {
        private static readonly DateTime MinimumBirthDate = new DateTime(2014, 1, 1);
        public StudentUpdateValidator()
        {
        RuleFor(x => x.FirstName)
            .Length(2, 15).WithMessage("Please specify a valid first name")
            .When(s => !string.IsNullOrEmpty(s.FirstName));  

      
        RuleFor(x => x.LastName)
            .Length(2, 15).WithMessage("Please specify a valid last name")
            .When(s => !string.IsNullOrEmpty(s.LastName));  

      
        RuleFor(x => x.StudentEmail)
            .EmailAddress().WithMessage("Please specify a valid email address")
            .When(s => !string.IsNullOrEmpty(s.StudentEmail));  

        
        RuleFor(x => x.StudentPhone)
            .Length(10).WithMessage("Please specify a valid phone number")
            .When(s => !string.IsNullOrEmpty(s.StudentPhone));  

        
        RuleFor(x => x.StudentGender)
            .Must(gender => gender == Gender.MALE || gender == Gender.FEMALE || gender == Gender.OTHER || gender == null)
            .WithMessage("Gender must be Male, Female, or Other: ( 1/2/3 ).")
            .When(s => s.StudentGender.HasValue);  

        
        RuleFor(x => x.BirthDate)
            .LessThan(MinimumBirthDate).WithMessage("Please enter a valid birthdate")
            .When(s => s.BirthDate.HasValue);    
        }
    }
}