using Application.Dtos;
using Application.Helpers;
using Application.InfraInterfaces;
using Application.Services;
using Application.Validators;
using AutoMapper;
using Core.Entities;
using FluentValidation;
using FluentValidation.Results;
using FluentAssertions;

using Moq;

namespace Tests;

public class ProductLocationTests
{
    [Theory]
    [InlineData(-51, true)]
    [InlineData(-10, false)]
    [InlineData(0, false)]
    public async Task ProductLocationService_ChangeQuantity_ReturnExceptionWhenReturnedQuantityNegative(int quantity, bool expectException)
    {
        // Arrange
        var actionDto = new ActionDto
        {
            SourcePLocationId = "1",
            Quantity = quantity,
            EmployeeId = 1,
            WarehouseId = 1,
            Type = ActionEnum.ChangeQuantity
        };

        var productLocation = new ProductLocation
        {
            Quantity = 50
        };

        var productLocationRepositoryMock = new Mock<IProductLocationRepository>();
        productLocationRepositoryMock.Setup(repo => repo.GetProductLocationAsync(It.IsAny<string>()))
           .ReturnsAsync(productLocation);

        var validatorMock = new Mock<AbstractValidator<ActionDto>>();
        validatorMock.Setup(validator => validator.Validate(It.IsAny<ValidationContext<ActionDto>>()))
            .Returns(new ValidationResult());

        var mapperMock = new Mock<IMapper>();

        var productLocationService = new ProductLocationService(
            productLocationRepositoryMock.Object,
            Mock.Of<ILogRepository>(),
            mapperMock.Object,
            validatorMock.Object
        );

        // Act
        if (expectException)
        {
            await Assert.ThrowsAsync<ApplicationException>(async () =>
            {
                await productLocationService.ChangeQuantity(actionDto);

                productLocationRepositoryMock.Verify(repo => repo.ChangeQuantity(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
            });
        }
        else
        {
            await productLocationService.ChangeQuantity(actionDto);

            productLocationRepositoryMock.Verify(repo => repo.ChangeQuantity(It.IsAny<string>(), It.IsAny<int>()), Times.Once);
        }

        productLocationRepositoryMock.Verify(repo => repo.GetProductLocationAsync(It.IsAny<string>()), Times.Once);

    }


    [Theory]
    [InlineData(ActionEnum.MoveToNewLocation, "sourceId", null, 10)]
    [InlineData(ActionEnum.MoveToExistingLocation, "sourceId", "destinationId", 5)]
    [InlineData(ActionEnum.ChangeQuantity, "sourceId", null, 5)]
    public async Task ProductLocationService_MoveQuantityAsync_VerifyRepoInvokation(ActionEnum type, string sourcePLocationId, string destinationPLocationId, int quantity)
    {
        // Arrange
        var actionDto = new ActionDto
        {
            Type = type,
            SourcePLocationId = sourcePLocationId,
            DestinationPLocationId = destinationPLocationId,
            Quantity = quantity
        };

        var productLocationRepositoryMock = new Mock<IProductLocationRepository>();
        var mapperMock = new Mock<IMapper>();

        var validationResult = new ValidationResult(); // Assume validation passes
        var validatorMock = new Mock<AbstractValidator<ActionDto>>();
        validatorMock.Setup(validator => validator.Validate(It.IsAny<ValidationContext<ActionDto>>()))
            .Returns(validationResult);

        var productLocationService = new ProductLocationService(
            productLocationRepositoryMock.Object,
            Mock.Of<ILogRepository>(),
            mapperMock.Object,
            validatorMock.Object
        );

        // Act and Assert
        await productLocationService.MoveQuantityAsync(actionDto);

        // Assert: Check interactions with dependencies based on the actionDto.Type
        if (actionDto.Type == ActionEnum.MoveToNewLocation)
        {
            // Verify that MoveToNewLocationAsync was called with the expected arguments
            productLocationRepositoryMock.Verify(repo => repo.MoveToNewLocationAsync(It.IsAny<ProductLocation>(), actionDto.SourcePLocationId), Times.Once);
        }
        else if (actionDto.Type == ActionEnum.MoveToExistingLocation)
        {
            // Verify that MoveToExistingLocationAsync was called with the expected arguments
            productLocationRepositoryMock.Verify(repo => repo.MoveToExistingLocationAsync(actionDto.SourcePLocationId, actionDto.DestinationPLocationId, actionDto.Quantity), Times.Once);
        }
        else
        {
            // Verify that ChangeQuantity was called with the expected arguments
            productLocationRepositoryMock.Verify(repo => repo.ChangeQuantity(actionDto.SourcePLocationId, actionDto.Quantity), Times.Never);
        }
    }


[Theory]
[InlineData(1, 1, "LocationId1", "SKU001", null, null, 10, ActionEnum.CreateFromScratch, true)] // Minimum required properties for a valid ActionDto
[InlineData(0, 2, "LocationId2", "SKU002", "SourcePLocation2", "DestinationPLocation2", 20, ActionEnum.CreateFromScratch, false)] // Invalid employeeId
[InlineData(3, 3, null, "SKU003", "SourcePLocation3", "DestinationPLocation3", 30, ActionEnum.CreateFromScratch, false)] // Must have a locationId

[InlineData(1, 1, null, null, "SourcePLocation1", "DestinationPLocation1", 10, ActionEnum.MoveToExistingLocation, true)] // Minimum required properties for a valid ActionDto
[InlineData(0, 2, "LocationId2", "SKU002", null, "DestinationPLocation2", 20, ActionEnum.MoveToExistingLocation, false)] // Invalid employeeId
[InlineData(3, 3, "LocationId3", "SKU003", "SourcePLocation3", "DestinationPLocation3", 0, ActionEnum.MoveToExistingLocation, false)] // Must have a quantity

[InlineData(1, 1, null, null, "SourcePLocation1", null, 10, ActionEnum.ChangeQuantity, true)] // Minimum required properties for a valid ActionDto
[InlineData(0, 2, "LocationId2", "SKU002", "SourcePLocation2", "DestinationPLocation2", 20, ActionEnum.ChangeQuantity, false)] // Invalid employeeId
[InlineData(3, 3, null, "SKU003", "SourcePLocation3", "DestinationPLocation3", 0, ActionEnum.ChangeQuantity, false)] // Requires a non-zero quantity

[InlineData(1, 1, "LocationId1", null, "SourcePLocation1", null, 10, ActionEnum.MoveToNewLocation, true)] // Minimum required properties for a valid ActionDto
[InlineData(0, 2, "LocationId2", "SKU002", "SourcePLocation2", null, 20, ActionEnum.MoveToNewLocation, false)] // Invalid employeeId
[InlineData(3, 3, "LocationId3", "SKU003", "SourcePLocation2", "DestinationPLocation3", 0, ActionEnum.MoveToNewLocation, false)] // Requires a non-zero quantity

// Add more test cases as needed
public void Validator_ActionDto_ShouldValidateCorrectly(int employeeId, int warehouseId, string locationId, string productSKU, string sourcePLocationId, string destinationPLocationId, int quantity, ActionEnum type, bool expectedIsValid)
{
    // Arrange
    var actionDto = new ActionDto
    {
        EmployeeId = employeeId,
        WarehouseId = warehouseId,
        LocationId = locationId,
        ProductSKU = productSKU,
        SourcePLocationId = sourcePLocationId,
        DestinationPLocationId = destinationPLocationId,
        Quantity = quantity,
        Type = type
        // Add other required properties for a valid ActionDto
    };

    var validator = new ActionDtoValidator();

    // Act
    var validationResult = validator.Validate(actionDto);

    // Assert
    Assert.Equal(expectedIsValid, validationResult.IsValid);
}

[Fact]
public async Task ProductLocationService_GetProductLocationAsync_ShouldMapToProductLocationDto()
{
    // Arrange
    var productLocationId = "1";
    var lastUpdated = DateTime.UtcNow;
    var productLocation = new ProductLocation
    {
        ProductLocationId = productLocationId,
        ProductSKU = "ABC123",
        LocationId = "Location1",
        Quantity = 10,
        LastUpdated = lastUpdated,
        WarehouseId = 1
        // Add other properties as needed
    };

    var expectedProductLocationDto = new ProductLocationDto
    {
        ProductLocationId = productLocationId,
        ProductSku = "ABC123",
        LocationId = "Location1",
        Quantity = 10,
        LastUpdated = lastUpdated,
        WarehouseId = 1
        // Add other properties as needed
    };

    var productLocationRepositoryMock = new Mock<IProductLocationRepository>();
    productLocationRepositoryMock.Setup(repo => repo.GetProductLocationAsync(productLocationId))
        .ReturnsAsync(productLocation);

    var mapperMock = new Mock<IMapper>();
    
    // Set up mapper to return expected DTO based on the input entity
    mapperMock.Setup(mapper => mapper.Map<ProductLocationDto>(It.IsAny<ProductLocation>()))
        .Returns<ProductLocation>(inputEntity => new ProductLocationDto
        {
            ProductLocationId = inputEntity.ProductLocationId,
            ProductSku = inputEntity.ProductSKU,
            LocationId = inputEntity.LocationId,
            Quantity = inputEntity.Quantity,
            LastUpdated = inputEntity.LastUpdated,
            WarehouseId = inputEntity.WarehouseId
            // Add other properties as needed
        });

    var productLocationService = new ProductLocationService(
        productLocationRepositoryMock.Object,
        Mock.Of<ILogRepository>(),
        mapperMock.Object,
        Mock.Of<AbstractValidator<ActionDto>>()
    );

    // Act
    var result = await productLocationService.GetProductLocationAsync(productLocationId);

    // Assert
    result.Should().BeEquivalentTo(expectedProductLocationDto);
    result.Should().BeOfType<ProductLocationDto>();
}




}



