using Application.Dtos;
using Application.Helpers;
using FluentValidation;

namespace Application.Validators;

public class ActionDtoValidator : AbstractValidator<ActionDto>
{
    public ActionDtoValidator()
    {
        RuleFor(action => action.EmployeeId)
            .GreaterThan(0)
            .WithMessage("Employee id is required.");

        RuleFor(action => action.WarehouseId)
            .GreaterThan(0)
            .WithMessage("Warehouse id is required.");

        When(action => action.Type == ActionEnum.CreateFromScratch, () =>
        {
            RuleFor(action => action.LocationId)
           .NotNull()
           .WithMessage("Source product location is required.");

            RuleFor(action => action.ProductSKU)
            .NotNull()
            .WithMessage("Product SKU is required.");

             RuleFor(action => action.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");

        });

        When(action => action.Type == ActionEnum.MoveToExistingLocation, () =>
        {
            RuleFor(action => action.SourcePLocationId)
            .NotNull()
            .WithMessage("Source product location is required.");

            RuleFor(action => action.DestinationPLocationId)
            .NotNull()
            .WithMessage("Destination product location is required.");

             RuleFor(action => action.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");

        });

        When(action => action.Type == ActionEnum.ChangeQuantity, () =>
        {
            RuleFor(action => action.SourcePLocationId)
            .NotNull()
            .WithMessage("Source product location is required.");

            RuleFor(action => action.Quantity)
            .NotEmpty()
            .WithMessage("Quantity is required.");
        });

        When(action => action.Type == ActionEnum.MoveToNewLocation, () =>
        {
            RuleFor(action => action.SourcePLocationId)
            .NotNull()
            .WithMessage("Source product location is required.");

            RuleFor(action => action.LocationId)
            .NotNull()
            .WithMessage("Source product location is required.");

             RuleFor(action => action.Quantity)
            .GreaterThan(0)
            .WithMessage("Quantity must be greater than 0.");
        });


    }
}