﻿using FluentValidation;
using SchoolApi.API.DTOs;
using SchoolApi.Business.Models;

namespace SchoolApi.API.Validators
{
    public class StudentValidator:AbstractValidator<StudentPostDto>
    {
        public StudentValidator()
        {
            RuleFor(x => x.FirstName)
                .NotNull().WithMessage("First name cannot be null.")
                .NotEmpty().WithMessage("First name cannot be empty.")
                 .Length(2, 15).WithMessage("First name must be between 2 and 15 characters long.");

            RuleFor(x => x.LastName)
                .NotNull().WithMessage("Last name cannot be null.")
                .NotEmpty().WithMessage("Last name cannot be empty.")
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
                .NotNull().WithMessage("Birth date cannot be null.");
        }
    }
    public class StudentUpdateValidator : AbstractValidator<UpdateStudentDto>
    {
        public StudentUpdateValidator()
        {
            RuleFor(x => x.FirstName).Length(2, 15).When(x => x.FirstName != null).WithMessage("Please specify a valid first name").When(s => !string.IsNullOrEmpty(s.FirstName));
            RuleFor(x => x.LastName).Length(2, 15).When(x => x.LastName != null).WithMessage("Please specify a valid last name").When(s => !string.IsNullOrEmpty(s.LastName));
            RuleFor(x => x.StudentEmail).EmailAddress().When(x => x.StudentEmail != null).WithMessage("Please specify a valid email").When(s => !string.IsNullOrEmpty(s.StudentEmail));
            RuleFor(x => x.StudentPhone).Length(10).When(x => x.StudentPhone != null).WithMessage("Please specify a valid phone number").When(s => !string.IsNullOrEmpty(s.StudentPhone));
            RuleFor(x => x.StudentGender)
                .Must(gender => gender == Gender.MALE || gender == Gender.FEMALE || gender == Gender.OTHER || gender == null)
                .WithMessage("Gender must be Male, Female or Other: ( 1/2/3 ).").When(s => !string.IsNullOrEmpty(s.StudentGender.ToString()));
            RuleFor(x => x.BirthDate).Must(date => date > DateTime.Now).WithMessage("Please enter a valid date").When(s => !string.IsNullOrEmpty(s.BirthDate.ToString()));
        }
    }

}
