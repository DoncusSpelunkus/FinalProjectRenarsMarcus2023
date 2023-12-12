using System.Text;
using Application.Dtos;
using Application.helpers;
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

    cfg.CreateMap<ProductLocation, ProductLocationDto>()
        .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.LastUpdated.ToString("yyyy-MM-ddTHH:mm:ss.fff")))
        .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
        .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));


    cfg.CreateMap<Employee, UserDto>();
    cfg.CreateMap<UserDto, Employee>();
    cfg.CreateMap<Employee, UserDto>()
        .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name));
    cfg.CreateMap<CreateProductLocationDto, ProductLocation>();

    cfg.CreateMap<Shipment, ShipmentDto>()
        .ForMember(dest => dest.ShipmentDetails, opt => opt.MapFrom(src => src.ShipmentDetails));
    cfg.CreateMap<ShipmentDto, Shipment>()
        .ForMember(dest => dest.ShipmentDetails, opt => opt.MapFrom(src => src.ShipmentDetails));

    cfg.CreateMap<ShipmentDetail, ShipmentDetailDto>();
    cfg.CreateMap<ShipmentDetailDto, ShipmentDetail>();

    cfg.CreateMap<Brand, BrandDto>();
    cfg.CreateMap<BrandDto, Brand>();

    cfg.CreateMap<Log, LogDto>();
    cfg.CreateMap<LogDto, Log>();

    cfg.CreateMap<Warehouse, WarehouseDto>();
    cfg.CreateMap<WarehouseDto, Warehouse>();
});

IMapper mapper = mapperConfiguration.CreateMapper();
builder.Services.AddSingleton(mapper);

Application.DependencyResolver.DependencyResolverService.RegisterApplicationLayer(builder.Services); // part of the dependency line for application 
Infrastructure.DependencyResolver.DependencyResolverService.RegisterInfrastructureLayer(builder.Services);// part of the dependency line  for infrastructure


builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
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
                    || path.StartsWithSegments("/SocketShipments")
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

builder.Services.AddDbContext<DbContextManagement>(options => options.UseSqlite(

    "Data source=db.db"
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

await app.RunAsync();
