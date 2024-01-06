using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class AddToShipmentValidator : AbstractValidator<AddToShipmentDetails>
{
    public AddToShipmentValidator(){
        RuleForEach(detail => detail.ShipmentDetails).SetValidator(new ShipmentDetailDtoValidator());
    }
}