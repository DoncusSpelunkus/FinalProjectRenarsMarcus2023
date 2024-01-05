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

public class LocationTests
{
    [Fact]
    public async Task LocationService_GetLocationAsync_ShouldMapToLocationDto()
    {
        // Arrange
        var locationId = "1";
        var warehouseId = 1;
        var locations = new List<Location>
    {
        new() {
            LocationId = locationId,
            Aisle = 3,
            Rack = 3,
            Shelf = 4,
            Bin = 5,
            WarehouseId = warehouseId
            // Add other properties as needed
        },
        // Add more locations as needed
    };

        var expectedLocationDtoList = locations.Select(inputEntity => new LocationDto
        {
            LocationId = inputEntity.LocationId,
            Aisle = 3,
            Rack = 3,
            Shelf = 4,
            Bin = 5,
            WarehouseId = inputEntity.WarehouseId
            // Add other properties as needed
        }).ToList();

        var locationRepositoryMock = new Mock<ILocationRepository>();
        locationRepositoryMock.Setup(repo => repo.GetLocationsByWarehouseAsync(warehouseId))
            .ReturnsAsync(locations);

        var mapperMock = new Mock<IMapper>();

        // Set up mapper to return expected DTO list based on the input entities
        mapperMock.Setup(mapper => mapper.Map<List<LocationDto>>(It.IsAny<List<Location>>()))
            .Returns<List<Location>>(inputEntities => inputEntities.Select(inputEntity => new LocationDto
            {
                LocationId = inputEntity.LocationId,
                Aisle = inputEntity.Aisle,
                Rack = inputEntity.Rack,
                Shelf = inputEntity.Shelf,
                Bin = inputEntity.Bin,
                WarehouseId = inputEntity.WarehouseId
                // Add other properties as needed
            }).ToList());

        var locationService = new LocationService(
            locationRepositoryMock.Object,
            mapperMock.Object,
            Mock.Of<AbstractValidator<LocationDto>>()
        );

        // Act
        var result = await locationService.GetLocationsByWarehouseAsync(warehouseId);

        // Assert
        result.Should().BeEquivalentTo(expectedLocationDtoList);
        result.Should().BeOfType<List<LocationDto>>();
    }

    [Fact]
    public async Task LocationService_CreateLocationAsync_ShouldCreateAndMapToLocationDto()
    {
        // Arrange
        var locationDto = new LocationDto
        {
            LocationId = "1",
            Aisle = 3,
            Rack = 3,
            Shelf = 4,
            Bin = 5,
            WarehouseId = 1
            // Add other properties as needed
        };

        var location = new Location
        {
            LocationId = locationDto.LocationId,
            Aisle = locationDto.Aisle,
            Rack = locationDto.Rack,
            Shelf = locationDto.Shelf,
            Bin = locationDto.Bin,
            WarehouseId = locationDto.WarehouseId
            // Add other properties as needed
        };

        var createdLocation = new Location
        {
            LocationId = locationDto.LocationId,
            Aisle = locationDto.Aisle,
            Rack = locationDto.Rack,
            Shelf = locationDto.Shelf,
            Bin = locationDto.Bin,
            WarehouseId = locationDto.WarehouseId
            // Add other properties as needed
        };

        var locationRepositoryMock = new Mock<ILocationRepository>();
        locationRepositoryMock.Setup(repo => repo.CreateLocationAsync(It.IsAny<Location>()))
            .ReturnsAsync(createdLocation);

        var validatorMock = new Mock<AbstractValidator<LocationDto>>();
        validatorMock.Setup(validator => validator.Validate(locationDto))
            .Returns(new ValidationResult());

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(mapper => mapper.Map<Location>(locationDto))
            .Returns(location);
        mapperMock.Setup(mapper => mapper.Map<LocationDto>(createdLocation))
            .Returns(locationDto);

        var locationService = new LocationService(
            locationRepositoryMock.Object,
            mapperMock.Object,
            validatorMock.Object
        );

        // Act
        var result = await locationService.CreateLocationAsync(locationDto);

        // Assert
        result.Should().BeEquivalentTo(locationDto);
    }


    [Fact]
    public async Task LocationService_UpdateLocationAsync_ShouldUpdateAndMapToLocationDto()
    {
        // Arrange
        var locationDto = new LocationDto
        {
            LocationId = "1",
            Aisle = 3,
            Rack = 3,
            Shelf = 4,
            Bin = 5,
            WarehouseId = 1
            // Add other properties as needed
        };

        var location = new Location
        {
            LocationId = locationDto.LocationId,
            Aisle = locationDto.Aisle,
            Rack = locationDto.Rack,
            Shelf = locationDto.Shelf,
            Bin = locationDto.Bin,
            WarehouseId = locationDto.WarehouseId
            // Add other properties as needed
        };

        var updatedLocation = new Location
        {
            LocationId = locationDto.LocationId,
            Aisle = locationDto.Aisle + 1, // Assume an update in aisle
            Rack = locationDto.Rack,
            Shelf = locationDto.Shelf,
            Bin = locationDto.Bin,
            WarehouseId = locationDto.WarehouseId
            // Add other properties as needed
        };

        var locationRepositoryMock = new Mock<ILocationRepository>();
        locationRepositoryMock.Setup(repo => repo.UpdateLocationAsync(It.IsAny<Location>()))
            .ReturnsAsync(updatedLocation);

        var validationResult = new ValidationResult(); // Assume validation passes
        var validatorMock = new Mock<AbstractValidator<LocationDto>>();
        validatorMock.Setup(validator => validator.Validate(It.IsAny<ValidationContext<LocationDto>>()))
            .Returns(validationResult);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(mapper => mapper.Map<Location>(locationDto))
            .Returns(location);
        mapperMock.Setup(mapper => mapper.Map<LocationDto>(updatedLocation))
            .Returns(locationDto);

        var locationService = new LocationService(
            locationRepositoryMock.Object,
            mapperMock.Object,
            validatorMock.Object
        );

        // Act
        var result = await locationService.UpdateLocationAsync(locationDto);

        // Assert
        result.Should().BeEquivalentTo(locationDto);
        locationRepositoryMock.Verify(repo => repo.UpdateLocationAsync(location), Times.Once);
    }

    [Theory]
    [InlineData(null, 1, 1, 1, 1, 1, true)] // Valid data
    [InlineData("", 1, 1, 1, 1, 1, true)]   // Valid data
    [InlineData("1", 0, 1, 1, 1, 1, false)]   // Invalid Aisle
    [InlineData("1", 1, 0, 1, 1, 1, false)]   // Invalid Rack
    [InlineData("1", 1, 1, 0, 1, 1, false)]   // Invalid Shelf
    [InlineData("1", 1, 1, 1, 0, 1, false)]   // Invalid Bin
    [InlineData("1", 1, 1, 1, 1, 0, false)]   // Invalid WarehouseId
    public void Validate_LocationDto_ShouldValidateCorrectly(string locationId, int aisle, int rack, int shelf, int bin, int warehouseId, bool expected)
    {
        // Arrange
        var locationDto = new LocationDto
        {
            LocationId = locationId,
            Aisle = aisle,
            Rack = rack,
            Shelf = shelf,
            Bin = bin,
            WarehouseId = warehouseId
        };

        var validator = new LocationDtoValidator();

        // Act
        var result = validator.Validate(locationDto);

        // Assert
        result.IsValid.Should().Be(expected);
    }


}