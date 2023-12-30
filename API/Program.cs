using System.Text;
using Application.Dtos;
using Application.helpers;
using Infrastructure.helpers;
using Application.InfraInterfaces;
using Application.IServices;
using Application.Services;
using Application.Validators;
using AutoMapper;
using Core.Entities;
using FluentValidation;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using sockets;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true;
});

builder.Services.AddLogging(builder =>
    {
        builder.AddConsole(); // You can add other logging providers as needed
        builder.AddDebug();
    });

var mapperConfiguration = new MapperConfiguration(cfg =>
{
    cfg.CreateMap<Product, ProductDto>();
    cfg.CreateMap<ProductDto, Product>();

    cfg.CreateMap<Location, LocationDto>();
    cfg.CreateMap<LocationDto, Location>();

    cfg.CreateMap<TypeDto, ProductType>();
    cfg.CreateMap<ProductType, TypeDto>();

    cfg.CreateMap<ProductLocation, ProductLocationDto>();
    cfg.CreateMap<ProductLocationDto, ProductLocation>();

    cfg.CreateMap<ProductLocation, ProductLocationDto>();


    cfg.CreateMap<Employee, UserDto>();
    cfg.CreateMap<UserDto, Employee>();
    cfg.CreateMap<Employee, UserDto>()
        .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));


    cfg.CreateMap<Shipment, ShipmentDto>()
        .ForMember(dest => dest.ShipmentDetails, opt => opt.MapFrom(src => src.ShipmentDetails));
    cfg.CreateMap<ShipmentDto, Shipment>()
        .ForMember(dest => dest.ShipmentDetails, opt => opt.MapFrom(src => src.ShipmentDetails));

    cfg.CreateMap<ShipmentDetail, ShipmentDetailDto>();
    cfg.CreateMap<ShipmentDetailDto, ShipmentDetail>();

    cfg.CreateMap<Brand, BrandDto>();
    cfg.CreateMap<BrandDto, Brand>();

    cfg.CreateMap<Log, MoveLogDto>();
    cfg.CreateMap<MoveLogDto, Log>();

    cfg.CreateMap<ProductLocation, ActionDto>();
    cfg.CreateMap<ActionDto, ProductLocation>();

    cfg.CreateMap<Warehouse, WarehouseDto>();
    cfg.CreateMap<WarehouseDto, Warehouse>();
});

IMapper mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);

Application.DependencyResolver.DependencyResolverService.RegisterApplicationLayer(builder.Services); // part of the dependency line for application 
Infrastructure.DependencyResolver.DependencyResolverService.RegisterInfrastructureLayer(builder.Services);// part of the dependency line  for infrastructure

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
builder.Services.Configure<InfastructureSettings>(builder.Configuration.GetSection("ConnectionStrings"));
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;

                if (
                    !string.IsNullOrEmpty(accessToken) 
                    && path.StartsWithSegments("/SocketInventory")
                    || path.StartsWithSegments("/SocketUserManagement")
                    || path.StartsWithSegments("/SocketShipment")
                    || path.StartsWithSegments("/SocketLogs")
                    || path.StartsWithSegments("/SocketAuth")
                    )

                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    builder.Configuration.GetValue<string>("AppSettings:Secret")
                )),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });

var _connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DbContextManagement>(options => 
options.UseMySql(
    _connectionString,
    ServerVersion.AutoDetect(_connectionString)
    ));

//builder.Services.AddScoped<WarehouseRepository>(); // check with this later 

var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors(options =>
{
    options.SetIsOriginAllowed(origin => true)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
});

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<InventorySocket>("/SocketInventory").RequireAuthorization();
app.MapHub<UserManagementSocket>("/SocketUserManagement").RequireAuthorization();
app.MapHub<ShipmentSocket>("/SocketShipment").RequireAuthorization();
app.MapHub<LogsSocket>("/SocketLogs").RequireAuthorization();
app.MapHub<AuthSocket>("/SocketAuth").RequireAuthorization();

await app.RunAsync();
