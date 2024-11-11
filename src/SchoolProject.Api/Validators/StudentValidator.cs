using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using SchoolProject.Api.DTOs;
using SchoolProject.Buisness.Models;

namespace SchoolProject.Api.Validators
{
    public class StudentValidator:AbstractValidator<StudentPostDto>
    {
        
     private static readonly DateTime MinimumBirthDate = new DateTime(2014, 1, 1);
        public StudentValidator()
        {

            RuleFor(x => x.FirstName)
                .NotNull().WithMessage("First name cannot be null.")
                .NotEmpty().WithMessage("First name cannot be empty.")
                .NotEqual("string").WithMessage("First name can not be empty.")
                 .Length(2, 15).WithMessage("First name must be between 2 and 15 characters long.");

            RuleFor(x => x.LastName)
                .NotNull().WithMessage("Last name cannot be null.")
                .NotEmpty().WithMessage("Last name cannot be empty.")
                .NotEqual("string").WithMessage("First name can not be empty.")
                .Length(2, 15).WithMessage("Last name must be between 2 and 15 characters long.");

            RuleFor(x => x.StudentEmail)
                .NotNull().WithMessage("Email cannot be null.")
                .NotEmpty().WithMessage("Email cannot be empty.")
                .EmailAddress().WithMessage("Please specify a valid email address.");

            RuleFor(x => x.StudentPhone)
                .NotNull().WithMessage("Phone number cannot be null.")
                .NotEmpty().WithMessage("Phone number cannot be empty.")
                .Length(10).WithMessage("Phone number must be exactly 10 digits long.");

            RuleFor(x => x.StudentGender)
                .Must(gender => gender == Gender.MALE || gender == Gender.FEMALE || gender == Gender.OTHER)
                .WithMessage("Gender must be Male, Female, or Other (1/2/3).");

            RuleFor(x => x.BirthDate)
                .NotNull().WithMessage("Birth date cannot be null.")
                .LessThan(MinimumBirthDate).WithMessage("Birth year should be grater than 2014");
        }
    }
   
    
}