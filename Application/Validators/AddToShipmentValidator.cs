using Application.Dtos;
using Application.Helpers;
using FluentValidation;

namespace Application.Validators;

public class AddToShipmentValidator : AbstractValidator<AddToShipmentDetails>
{
    public AddToShipmentValidator(){
        RuleForEach(detail => detail.ShipmentDetails).SetValidator(new ShipmentDetailDtoValidator());
    }
}