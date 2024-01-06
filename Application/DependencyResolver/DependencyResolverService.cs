using Application.Dtos;
using Application.IServices;
using Application.Services;
using Application.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Application.DependencyResolver;

public static class DependencyResolverService
{
    public static void RegisterApplicationLayer(IServiceCollection services)
    {
        services.AddScoped<AbstractValidator<ProductDto>, ProductValidator>();
        services.AddScoped<AbstractValidator<LocationDto>, LocationDtoValidator>();
        services.AddScoped<AbstractValidator<ActionDto>, ActionDtoValidator>();
        services.AddScoped<AbstractValidator<ShipmentDto>, ShipmentDtoValidator>();
        services.AddScoped<AbstractValidator<ShipmentDetailDto>, ShipmentDetailDtoValidator>();
        services.AddScoped<AbstractValidator<TypeDto>, TypeDtoValidator>();
        services.AddScoped<AbstractValidator<BrandDto>, BrandDtoValidator>();
        services.AddScoped<AbstractValidator<LoginDto>, LoginValidator>();
        services.AddScoped<AbstractValidator<UserDto>, UserDtoValidator>();
        services.AddScoped<AbstractValidator<string>, PasswordValidator>();
        services.AddValidatorsFromAssemblyContaining<ShipmentDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<ShipmentDetailDtoValidator>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IWarehouseService, WarehouseService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IProductLocationService, ProductLocationService>();
        services.AddScoped<IShipmentService, ShipmentService>(); // for both class's 
        services.AddScoped<IBrandService, BrandService>(); // for both class's 
        services.AddScoped<ITypeService, TypeService>(); // for both class's 
        services.AddScoped<IBackupService, BackupService>();
        services.AddScoped<IEmailService, GmailEmailService>();




    }
}