using Application.Dtos;
using FluentValidation;

namespace Application.Validators
{
    public class WarehouseValidator : AbstractValidator<WarehouseDto>
    {
        public WarehouseValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
            RuleFor(x => x.Capacity).GreaterThanOrEqualTo(0).WithMessage("Capacity must be greater than or equal to 0");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required");
        }
    }
}