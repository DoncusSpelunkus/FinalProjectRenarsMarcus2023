
using Application.InfraInterfaces;
using Infrastructure.Repos;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyResolver;

public static class DependencyResolverService
{
    public static void RegisterInfrastructureLayer(IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IEmployeeRepository, EmployeeRepository>();
        serviceCollection.AddScoped<IWarehouseRepository, WarehouseRepository>();
        serviceCollection.AddScoped<IProductRepository, ProductRepository>();
        serviceCollection.AddScoped<ILocationRepository, LocationRepository>();
        serviceCollection.AddScoped<IShipmentRepository, ShipmentRepository>();
        serviceCollection.AddScoped<IProductLocationRepository, ProductLocationRepository>();
        serviceCollection.AddScoped<IBrandRepository, BrandRepository>();
        serviceCollection.AddScoped<ITypeRepository, TypeRepository>();
        serviceCollection.AddScoped<ILogRepository, LogRepository>();
    }
}