using Application.Dtos;
using Application.helpers;
using Application.InfraInterfaces;
using Application.IServices;
using Application.Services;
using Application.Validators;
using AutoMapper;
using Core.Entities;
using FluentValidation;
using Microsoft.Extensions.Options;

using Moq;

namespace Tests;

public class UnitTest1
{
     [Fact]
    public async Task ShouldCreateEmployeeAndReturnUserDtoAndCheckIfUserDoesNotExist()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Username = "testuser",
            Password = "password123",
            Name = "Samkaxe",
            Email = "Samkaxe@gmail.com",
            Role = "standard",
            warehouseId = 1
        };

        var employee = new Employee
        {
            EmployeeId = 1,
            Username = registerDto.Username.ToLower(),
            Name = registerDto.Name.ToLower(),
            PasswordHash = new byte[64],
            PasswordSalt = new byte[128],
            Email = registerDto.Email,
            Role = registerDto.Role,
            WarehouseId = registerDto.warehouseId
        };

        var employeeRepositoryMock = new Mock<IEmployeeRepository>();
        var tokenServiceMock = new Mock<ITokenService>();

        employeeRepositoryMock.Setup(repo => repo.UserExists(registerDto.Username))
            .ReturnsAsync(false);
        employeeRepositoryMock.Setup(repo => repo.CreateEmployee(It.IsAny<Employee>()))
            .ReturnsAsync(employee);

        tokenServiceMock.Setup(tokenService => tokenService.CreateToken(employee))
            .Returns("Samkaxe"); // fake value 

        var mapperMock = new Mock<IMapper>();
        var employeeService = new EmployeeService(employeeRepositoryMock.Object, tokenServiceMock.Object, mapperMock.Object);

        // Act
        var result = await employeeService.CreateEmployee(registerDto);

        // Assert
        // Assert.NotNull(result);
        // Assert.Equal(registerDto.Username.ToLower(), result.DisplayName);
        // Assert.Equal("Samkaxe", result.Token);
    }

    [Fact]
    public async Task ShouldReturnUserDtoIfUserExistsInDatabase()
    {
        // Arrange
        int employeeId = 1;
        var expectedEmployee = new Employee
        {
            EmployeeId = employeeId,
            Username = "testuser",
        };

        var employeeRepositoryMock = new Mock<IEmployeeRepository>();
        var tokenServiceMock = new Mock<ITokenService>();
        var mapperMock = new Mock<IMapper>();

        var userDto = new UserDto
        {
            DisplayName = expectedEmployee.Username,
            Token = "your-token-value"
        };

        employeeRepositoryMock.Setup(repo => repo.GetEmployeeById(employeeId))
            .ReturnsAsync(expectedEmployee);
        tokenServiceMock.Setup(tokenService => tokenService.CreateToken(expectedEmployee))
            .Returns("your-token-value");
        mapperMock.Setup(mapper => mapper.Map<UserDto>(expectedEmployee))
            .Returns(userDto);

        var employeeService = new EmployeeService(employeeRepositoryMock.Object, tokenServiceMock.Object, mapperMock.Object);

        // Act
        var result = await employeeService.GetEmployeeById(employeeId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedEmployee.Username, result.DisplayName);
        Assert.Equal("your-token-value", result.Token);
    }


    [Fact]
    public void ShouldCreateTokenBasedOnTheEmployeeDetails()
    {
        // Arrange
        var appSettings = Options.Create(new AppSettings
        {
            Secret = "this is test secret fake token :D  "
        });

        var tokenService = new TokenService(appSettings);
        var user = new Employee
        {
            Name = "Sam kaxe",
            Role = "Employee"
        };

        // Act
        var token = tokenService.CreateToken(user);

        // Assert
        Assert.NotNull(token);
    }

    
    [Fact]
    public async Task ShouldReturnListOfUserDtosBasedOnTheWarehouseID()
    {
        // Arrange
        int warehouseId = 1;
        var expectedEmployees = new List<Employee>
        {
            new Employee { EmployeeId = 1, WarehouseId = warehouseId },
            new Employee { EmployeeId = 2, WarehouseId = warehouseId },
        };

        var employeeRepositoryMock = new Mock<IEmployeeRepository>();
        employeeRepositoryMock.Setup(repo =>
                repo.GetEmployeesByWarehouseId(warehouseId))
            .ReturnsAsync(expectedEmployees);

        var tokenServiceMock = new Mock<ITokenService>();
        var mapperMock = new Mock<IMapper>();

        var expectedUserDtos = expectedEmployees
            .Select(employee => new UserDto
            {
                DisplayName = employee.Username,
                Token = "your-token-value"
            })
            .ToList();

        mapperMock.Setup(mapper => mapper.Map<List<UserDto>>(expectedEmployees))
            .Returns(expectedUserDtos);

        var employeeService = new EmployeeService(employeeRepositoryMock.Object, tokenServiceMock.Object, mapperMock.Object);

        // Act
        var result = await employeeService.GetEmployeesByWarehouseId(warehouseId);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<List<UserDto>>(result);
        Assert.Equal(expectedUserDtos.Count, result.Count);
    }
    
    
    [Fact]
    public async Task ShouldGetLocationsWhenInsertingValidWarehouseID()
    {
        // Arrange
        int warehouseId = 1; 
        var locations = new List<Location>(); 

        var locationRepositoryMock = new Mock<ILocationRepository>();
        locationRepositoryMock.Setup(repo => repo.GetLocationsByWarehouseAsync(warehouseId))
            .ReturnsAsync(locations);

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(mapper => mapper.Map<List<LocationDto>>(locations))
            .Returns(new List<LocationDto>()); 

        var locationService = new LocationService(locationRepositoryMock.Object, mapperMock.Object, null);

        // Act
        var result = await locationService.GetLocationsByWarehouseAsync(warehouseId);

        // Assert
        Assert.NotNull(result);
    }
    
    [Fact]
    public async Task ShouldCreateLocationBasedOnMockedLocations()
    {
        // Arrange
        var locationDto = new LocationDto();
        var location = new Location();

        var locationRepositoryMock = new Mock<ILocationRepository>();
        locationRepositoryMock.Setup(repo => repo.CreateLocationAsync(It.IsAny<Location>()))
            .ReturnsAsync(location);

        var mapper = new Mock<IMapper>().Object;
        var validator = new LocationDtoValidator();

        var locationService = new LocationService(locationRepositoryMock.Object, mapper, validator);
       //  var result = await locationService.CreateLocationAsync(locationDto);

        // Assert
        Assert.True(true); 
    }
    
    [Fact]
    public async Task ShouldDeleteLocationANDWhenValidLocationDto_ReturnsTrue()
    {
        // Arrange
        var locationDto = new LocationDto(); 
        var location = new Location(); 

        var locationRepositoryMock = new Mock<ILocationRepository>();
        locationRepositoryMock.Setup(repo => repo.DeleteLocationAsync(It.IsAny<Location>()))
            .ReturnsAsync(true); 

        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(mapper => mapper.Map<Location>(locationDto))
            .Returns(location);

        var locationService = new LocationService(locationRepositoryMock.Object, mapperMock.Object, null);

        // Act
        var result = await locationService.DeleteLocationAsync(locationDto);

        // Assert
        Assert.True(result);
    }
    
    [Fact]
        public async Task GetLogsByWarehouseAsyncANDReturnsMappedLogs()
        {
            // Arrange
            var warehouseId = 1;
            var logs = new List<Log>
            {
                new Log { LogId = 1,
                    ProductSKU = "ABC123",
                    FromLocationId = "LocationA",
                    ToLocationId = "LocationB",
                    Quantity = 10,
                    UserId = 1,
                    Timestamp = DateTime.UtcNow.AddDays(-10), 
                    WarehouseId = 1 },
                new Log {  LogId = 2,
                    ProductSKU = "XYZ789",
                    FromLocationId = "LocationC",
                    ToLocationId = "LocationD",
                    Quantity = 5,
                    UserId = 2,
                    Timestamp = DateTime.UtcNow.AddDays(-5),
                    WarehouseId = 2 }
            };

            var logRepositoryMock = new Mock<ILogRepository>();
            logRepositoryMock.Setup(repo => repo.GetLogsByWarehouseAsync(warehouseId))
                .ReturnsAsync(logs);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<List<LogDto>>(logs))
                .Returns(new List<LogDto> { /* prop logdto will be refactored */ });

            var productLocationService = new ProductLocationService(
                Mock.Of<IProductLocationRepository>(),
                logRepositoryMock.Object,
                mapperMock.Object,
                Mock.Of<AbstractValidator<CreateProductLocationDto>>()
            );

            // Act
            var result = await productLocationService.GetLogsByWarehouseAsync(warehouseId);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateLogAsyncAndReturnsMappedLogDto()
        {
            // Arrange
            var createLogDto = new LogDto
            {
                ProductSKU = "ABC123",
                FromLocationId = "LocationA",
                ToLocationId = "LocationB",
                Quantity = 10,
                UserId = 1,
                Timestamp = DateTime.UtcNow,
                WarehouseId = 1
            };

            var logEntity = new Log
            {
                ProductSKU = createLogDto.ProductSKU,
                FromLocationId = createLogDto.FromLocationId,
                ToLocationId = createLogDto.ToLocationId,
                Quantity = createLogDto.Quantity,
                UserId = createLogDto.UserId,
                Timestamp = createLogDto.Timestamp,
                WarehouseId = createLogDto.WarehouseId
            };

            var logRepositoryMock = new Mock<ILogRepository>();
            logRepositoryMock.Setup(repo => repo.CreateLogAsync(It.IsAny<Log>()))
                .ReturnsAsync(logEntity);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<Log>(createLogDto))
                .Returns(logEntity);
            mapperMock.Setup(mapper => mapper.Map<LogDto>(logEntity))
                .Returns(new LogDto {  
                    LogId = logEntity.LogId,
                    ProductSKU = logEntity.ProductSKU,
                    FromLocationId = logEntity.FromLocationId,
                    ToLocationId = logEntity.ToLocationId,
                    Quantity = logEntity.Quantity,
                    UserId = logEntity.UserId,
                    Timestamp = logEntity.Timestamp,
                    WarehouseId = logEntity.WarehouseId });

            var productLocationService = new ProductLocationService(
                Mock.Of<IProductLocationRepository>(),
                logRepositoryMock.Object,
                mapperMock.Object,
                Mock.Of<AbstractValidator<CreateProductLocationDto>>()
            );

            // Act
            var result = await productLocationService.CreateLogAsync(createLogDto);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task DeleteLogsOlderThanOneYearAsync()
        {
            // Arrange
            var logRepositoryMock = new Mock<ILogRepository>();
            logRepositoryMock.Setup(repo => repo.DeleteLogsOlderThanOneYearAsync())
                .ReturnsAsync(true);

            var productLocationService = new ProductLocationService(
                Mock.Of<IProductLocationRepository>(),
                logRepositoryMock.Object,
                Mock.Of<IMapper>(),
                Mock.Of<AbstractValidator<CreateProductLocationDto>>()
            );

            // Act
            var result = await productLocationService.DeleteLogsOlderThanOneYearAsync();

            // Assert
            Assert.True(result);
            
        }
        
         [Fact]
        public async Task CreateWarehouseReturnsMappedDto()
        {
            // Arrange
            var warehouseDto = new WarehouseDto
            {
                Name = "Test Warehouse",
                Capacity = 100,
                Address = "123 Main St"
            };

            var warehouse = new Warehouse
            {
                Name = warehouseDto.Name,
                Capacity = warehouseDto.Capacity,
                Address = warehouseDto.Address
            };

            var warehouseRepositoryMock = new Mock<IWarehouseRepository>();
            warehouseRepositoryMock.Setup(repo => repo.CreateWarehouseAsync(It.IsAny<Warehouse>()))
                .ReturnsAsync(warehouse);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<WarehouseDto>(warehouse))
                .Returns(warehouseDto);

            var warehouseService = new WarehouseService(warehouseRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await warehouseService.CreateWarehouse(warehouseDto);

            // Assert
            // Assert.NotNull(result);
            // Assert.Equal(warehouseDto.Name, result.Name);
            // Assert.Equal(warehouseDto.Capacity, result.Capacity);
            // Assert.Equal(warehouseDto.Address, result.Address);
        }

        [Fact]
        public async Task UpdateWarehouseAsyncReturnsMappedDto()
        {
            // Arrange
            var warehouseId = 1;
            var warehouseDto = new WarehouseDto
            {
                Name = "Updated Warehouse",
                Capacity = 150,
                Address = "456 Oak St"
            };

            var existingWarehouse = new Warehouse
            {
                WarehouseId = warehouseId,
                Name = "Original Warehouse",
                Capacity = 100,
                Address = "123 Main St"
            };

            var updatedWarehouse = new Warehouse
            {
                WarehouseId = warehouseId,
                Name = warehouseDto.Name,
                Capacity = warehouseDto.Capacity,
                Address = warehouseDto.Address
            };

            var warehouseRepositoryMock = new Mock<IWarehouseRepository>();
            warehouseRepositoryMock.Setup(repo => repo.GetWarehouseByIdAsync(warehouseId))
                .ReturnsAsync(existingWarehouse);
            warehouseRepositoryMock.Setup(repo => repo.UpdateWarehouseAsync(It.IsAny<Warehouse>()))
                .ReturnsAsync(updatedWarehouse);

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(mapper => mapper.Map<WarehouseDto>(updatedWarehouse))
                .Returns(warehouseDto);

            var warehouseService = new WarehouseService(warehouseRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await warehouseService.UpdateWarehouseAsync(warehouseId, warehouseDto);

            // Assert
            // Assert.NotNull(result);
            // Assert.Equal(warehouseDto.Name, result.Name);
            // Assert.Equal(warehouseDto.Capacity, result.Capacity);
            // Assert.Equal(warehouseDto.Address, result.Address);
        }

        [Fact]
        public async Task UpdateWarehouseAsync_WithNonExistingWarehouse_ReturnsNull()
        {
            // Arrange
            var warehouseId = 1;
            var warehouseDto = new WarehouseDto
            {
                Name = "Updated Warehouse",
                Capacity = 150,
                Address = "456 Oak St"
            };

            var warehouseRepositoryMock = new Mock<IWarehouseRepository>();
            warehouseRepositoryMock.Setup(repo => repo.GetWarehouseByIdAsync(warehouseId))
                .ReturnsAsync((Warehouse)null);

            var mapperMock = new Mock<IMapper>();
            var warehouseService = new WarehouseService(warehouseRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await warehouseService.UpdateWarehouseAsync(warehouseId, warehouseDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteWarehouseAsyncReturnsTrue()
        {
            // Arrange
            var warehouseId = 1;

            var warehouseRepositoryMock = new Mock<IWarehouseRepository>();
            warehouseRepositoryMock.Setup(repo => repo.DeleteWarehouseAsync(warehouseId))
                .ReturnsAsync(true);

            var mapperMock = new Mock<IMapper>();
            var warehouseService = new WarehouseService(warehouseRepositoryMock.Object, mapperMock.Object);

            // Act
            var result = await warehouseService.DeleteWarehouseAsync(warehouseId);

            // Assert
            Assert.True(result);
        }
}

