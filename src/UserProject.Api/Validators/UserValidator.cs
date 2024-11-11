using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using UserProject.Api.DTOs;

namespace UserProject.Api.Validators
{
    public class UserValidator: AbstractValidator<UserRegistrationDto>
{
    public UserValidator()
    {
        RuleFor(user => user.UserName)
            .NotEmpty()
            .WithMessage("Username is required.")
            .Length(3, 50)
            .WithMessage("Username must be between 3 and 50 characters long.");

        RuleFor(user => user.UserEmail)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.");

        RuleFor(user => user.UserPassword)
            .NotEmpty()
            .WithMessage("Password is required.")
            .MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long.");

        RuleFor(user => user.IsAdmin)
            .NotNull()
            .WithMessage("IsAdmin must be specified.");
    }
           
    }
}