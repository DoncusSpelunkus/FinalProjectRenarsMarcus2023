using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class ShipmentDetailDtoValidator : AbstractValidator<ShipmentDetailDto>
{
    public ShipmentDetailDtoValidator()
    {
        RuleFor(detail => detail.ShipmentId).NotEmpty().GreaterThan(0);
        RuleFor(detail => detail.ProductSKU).NotEmpty();
        RuleFor(detail => detail.Quantity).NotEmpty().GreaterThan(0);
    }
}