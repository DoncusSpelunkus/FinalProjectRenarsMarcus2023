using System.Data;
using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class PasswordValidator : AbstractValidator<string> // Rules for userDTO validation
{
    public PasswordValidator()
    {
        RuleFor(stringPassword => stringPassword)

              .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
              .Matches("[0-9]").WithMessage("Password must contain at least one number.")
              .Matches("[a-z]").WithMessage("Password must contain at least one lowercase letter.")
              .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
              .Matches("[!@#$%^&*/]").WithMessage("Password must contain at least one special character.");
    }
}