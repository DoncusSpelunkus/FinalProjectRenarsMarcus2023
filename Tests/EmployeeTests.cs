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
using Application.IServices;

namespace Tests;

public class EmployeeTests
{

    [Theory]
    [InlineData("JohnDoe", "john.doe@example.com", "admin", 1, true)] // Minimum required properties for a valid UserDto
    [InlineData("", "john.doe@example.com", "admin", 1, false)] // Invalid empty username
    [InlineData("JohnDoe", "notanemail", "admin", 1, false)] // Invalid email address
    [InlineData("JohnDoe", "john.doe@example.com", "invalidRole", 1, false)] // Invalid role
    [InlineData("JohnDoe", "john.doe@example.com", "admin", 0, false)] // Invalid warehouse ID
    // Add more test cases as needed
    public void Validator_UserDto_ShouldValidateCorrectly(string username, string email, string role, int warehouseId, bool expectedIsValid)
    {
        // Arrange
        var userDto = new UserDto
        {
            Username = username,
            Email = email,
            Role = role,
            WarehouseId = warehouseId
            // Add other required properties for a valid UserDto
        };

        var validator = new UserDtoValidator();

        // Act
        var validationResult = validator.Validate(userDto);

        // Assert
        Assert.Equal(expectedIsValid, validationResult.IsValid);
    }

    [Fact]
    public async Task CreateEmployee_ValidUser_ReturnsUserDto()
    {
        // Arrange
        var userDto = new UserDto
        {
            Username = "JohnDoe",
            Email = "JohnDoe@gmail.dk",
            Name = "John Doe",
            Role = "admin",
            WarehouseId = 1
        };

        var validationResult = new ValidationResult(); // Assume validation passes
        var validatorMock = new Mock<AbstractValidator<UserDto>>();
        validatorMock.Setup(validator => validator.Validate(It.IsAny<ValidationContext<UserDto>>()))
            .Returns(validationResult);

        var mockEmailService = new Mock<IEmailService>();
        mockEmailService.Setup(e => e.SendTemporaryCredentials(It.IsAny<string>(), It.IsAny<string>()));

        var passwordValidationResult = new ValidationResult(); // Assume validation passes
        var passwordValidatorMock = new Mock<AbstractValidator<string>>();
        passwordValidatorMock.Setup(validator => validator.Validate(It.IsAny<ValidationContext<string>>()))
            .Returns(passwordValidationResult);

        var mockMapper = new Mock<IMapper>();
        mockMapper.Setup(m => m.Map<Employee>(It.IsAny<UserDto>()))
            .Returns<UserDto>(input => new Employee
            {
                EmployeeId = 1,
                Username = input.Username,
                PasswordHash = new byte[] { /* your byte array here */ },
                PasswordSalt = new byte[] { /* your byte array here */ },
                Name = input.Name,
                Email = input.Email,
                Role = input.Role,
                WarehouseId = input.WarehouseId
                // Other properties you want to set
            });

        mockMapper
            .Setup(m => m.Map<UserDto>(It.IsAny<Employee>()))
            .Returns<Employee>(employee => new UserDto
            {
                Username = employee.Username,
                Name = employee.Name,
                Email = employee.Email,
                Role = employee.Role,
                WarehouseId = employee.WarehouseId
                // Other properties you want to set
            });

        var mockEmployeeRepository = new Mock<IEmployeeRepository>();
        mockEmployeeRepository.Setup(r => r.UserExists(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        mockEmployeeRepository.Setup(r => r.CreateEmployee(It.IsAny<Employee>())).ReturnsAsync(new Employee());

        var employeeService = new EmployeeService(
            mockEmployeeRepository.Object,
            Mock.Of<ITokenService>(),
            mockMapper.Object,
            mockEmailService.Object,
            passwordValidatorMock.Object,
            validatorMock.Object, // Use this line instead of validatorMock.Object
            Mock.Of<AbstractValidator<LoginDto>>()
        );

        // Act
        var result = await employeeService.CreateEmployee(userDto);

        // Assert
        Assert.NotNull(result);
        // Add more specific assertions based on your expectations

        mockEmployeeRepository.Verify(r => r.UserExists(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        mockEmployeeRepository.Verify(r => r.CreateEmployee(It.IsAny<Employee>()), Times.Once);
    }



}