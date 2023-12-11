using System.Data;
using Application.Dtos;
using FluentValidation;
using Application.Helpers;

namespace Application.Validators;

public class UserDtoValidator : AbstractValidator<UserDto> // Rules for userDTO validation
{
    public UserDtoValidator()
    {
        RuleFor(user => user.Username)
            .NotEmpty().WithMessage("Username cannot be empty.")
            .MinimumLength(8).WithMessage("Username must be at least 8 characters long.");

        RuleFor(user => user.Email)
            .EmailAddress().WithMessage("Email address is not valid.");

        RuleFor(user => user.Role)
            .Matches("^(admin|sales|employee)$").WithMessage("Role must be valid, either 'admin', 'sales', or 'employee'.");

        RuleFor(user => user.WarehouseId)
            .GreaterThan(0).WithMessage("Warehouse ID must be greater than 0.");

    }

}