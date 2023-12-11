using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class TypeDtoValidator : AbstractValidator<TypeDto>
{
    public TypeDtoValidator()
    {
        RuleFor(type => type.Name)
            .NotEmpty().WithMessage("Type name cannot be empty.")
            .MaximumLength(255).WithMessage("Type name must not exceed 255 characters.");

        RuleFor(type => type.WarehouseId)
            .GreaterThan(0).WithMessage("WarehouseId must be greater than 0.");
    }
}