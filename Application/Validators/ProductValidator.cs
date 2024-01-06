using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class ProductValidator : AbstractValidator<ProductDto>
{
    public ProductValidator()
    {
        RuleFor(x => x.SKU).NotEmpty().WithMessage("SKU is required");
        RuleFor(x => x.BrandId).GreaterThan(0).WithMessage("BrandId must be greater than 0");
        RuleFor(x => x.TypeId).GreaterThan(0).WithMessage("TypeId must be greater than 0");
        RuleFor(x => x.WarehouseId).GreaterThan(0).WithMessage("WarehouseId must be greater than 0");
    }

}