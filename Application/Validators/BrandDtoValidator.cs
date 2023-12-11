using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class BrandDtoValidator : AbstractValidator<BrandDto>
{
    public BrandDtoValidator()
    {
        RuleFor(brand => brand.Name)
            .NotEmpty().WithMessage("Brand name cannot be empty.")
            .MaximumLength(255).WithMessage("Brand name must not exceed 255 characters.");

        RuleFor(brand => brand.WarehouseId)
            .GreaterThan(0).WithMessage("WarehouseId must be greater than 0.");
    }
}