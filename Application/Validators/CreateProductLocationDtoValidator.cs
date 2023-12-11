using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class CreateProductLocationDtoValidator : AbstractValidator<CreateProductLocationDto>
{
    public CreateProductLocationDtoValidator()
    {
        RuleFor(x => x.ProductSKU)
            .NotEmpty().WithMessage("Product SKU is required");

        RuleFor(x => x.LocationId)
            .NotEmpty().WithMessage("Location ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity must be greater than or equal to 0");

        RuleFor(x => x.WarehouseId)
            .NotEmpty().WithMessage("Warehouse ID is required");
    }
}