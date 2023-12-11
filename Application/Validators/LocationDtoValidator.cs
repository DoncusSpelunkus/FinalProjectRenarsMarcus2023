using Application.Dtos;
using FluentValidation;

namespace Application.Validators;

public class LocationDtoValidator: AbstractValidator<LocationDto>
{
    public LocationDtoValidator()
    {
        RuleFor(dto => dto.Aisle).NotEmpty().WithMessage("Aisle is required.");
        RuleFor(dto => dto.Rack).NotEmpty().WithMessage("Rack is required.");
        RuleFor(dto => dto.Shelf).NotEmpty().WithMessage("Shelf is required.");
        RuleFor(dto => dto.Bin).NotEmpty().WithMessage("Bin is required.");
        RuleFor(dto => dto.WarehouseId).GreaterThan(0).WithMessage("Invalid warehouse ID.");
    }
}