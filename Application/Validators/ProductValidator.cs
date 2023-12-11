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
    
    // todo : add this line when Development process is done 
    
    // RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
    // RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
    // RuleFor(x => x.Category).NotEmpty().WithMessage("Category is required");
    // RuleFor(x => x.Weight).GreaterThanOrEqualTo(0).WithMessage("Weight must be greater than or equal to 0");
    // RuleFor(x => x.Length).GreaterThanOrEqualTo(0).WithMessage("Length must be greater than or equal to 0");
    // RuleFor(x => x.Width).GreaterThanOrEqualTo(0).WithMessage("Width must be greater than or equal to 0");
    // RuleFor(x => x.Height).GreaterThanOrEqualTo(0).WithMessage("Height must be greater than or equal to 0");
    // RuleFor(x => x.SupplierName).NotEmpty().WithMessage("SupplierName is required");
    // RuleFor(x => x.SupplierContact).NotEmpty().WithMessage("SupplierContact is required");
}