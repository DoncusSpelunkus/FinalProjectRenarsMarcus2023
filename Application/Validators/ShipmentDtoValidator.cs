using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class ShipmentDtoValidator : AbstractValidator<ShipmentDto>
{
    public ShipmentDtoValidator()
    {
        RuleFor(shipment => shipment.WarehouseId).NotEmpty().GreaterThan(0);
        RuleFor(shipment => shipment.ShippedByEmployeeId).NotEmpty().GreaterThan(0);
        RuleForEach(shipment => shipment.ShipmentDetails).SetValidator(new ShipmentDetailDtoValidator());
    }
}