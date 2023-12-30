using System.Reflection;
using Application.helpers;
using Core.Entities;
using Infrastructure.helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;


namespace Infrastructure.Contexts;

public class DbContextManagement : DbContext
{

    private readonly InfastructureSettings _infastructureSettings;

    public DbContextManagement(DbContextOptions<DbContextManagement> options, IOptions<InfastructureSettings> appSettings) : base(options)
    {
         _infastructureSettings = appSettings.Value;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var _connectionString = _infastructureSettings.ConnectionString;
        Console.WriteLine(_connectionString);
        optionsBuilder.UseMySql(_connectionString, ServerVersion.AutoDetect(_connectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // EMPLOYEE
        modelBuilder.Entity<Employee>()
            .HasOne(e => e.Warehouse)
            .WithMany(w => w.Employees)
            .HasForeignKey(e => e.WarehouseId);

        modelBuilder.Entity<Employee>()
            .Property(e => e.EmployeeId)
            .ValueGeneratedOnAdd();

        // EMPLOYEE

        // LOCATION
        modelBuilder.Entity<Location>()
            .Property(e => e.LocationId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Location>()
            .HasOne(l => l.Warehouse)
            .WithMany(w => w.Locations)
            .HasForeignKey(l => l.WarehouseId);
        // LOCATION

        // PRODUCT
        modelBuilder.Entity<Product>()
            .HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.ProductType)
            .WithMany(pt => pt.Products)
            .HasForeignKey(p => p.TypeId);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Warehouse)
            .WithMany(w => w.Products)
            .HasForeignKey(p => p.WarehouseId);
        // PRODUCT

        // LOG
        modelBuilder.Entity<Log>()
            .HasOne(l => l.Employee)
            .WithMany(e => e.Logs)
            .HasForeignKey(l => l.UserId);

        modelBuilder.Entity<Log>()
            .Property(e => e.LogId)
            .ValueGeneratedOnAdd();
        // LOG

        // ORDER
        modelBuilder.Entity<Orders>()
            .HasOne(o => o.Warehouse)
            .WithMany(w => w.Orders)
            .HasForeignKey(o => o.WarehouseId);

        modelBuilder.Entity<Orders>()
            .Property(e => e.OrderId)
            .ValueGeneratedOnAdd();
        // ORDER

        // PRODUCT LOCATION
        modelBuilder.Entity<ProductLocation>()
            .HasOne(pl => pl.Warehouse)
            .WithMany(w => w.ProductLocations)
            .HasForeignKey(pl => pl.WarehouseId);

        modelBuilder.Entity<ProductLocation>()
            .Property(e => e.ProductLocationId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ProductLocation>()
            .HasOne(pl => pl.Product)
            .WithMany(p => p.ProductLocations)
            .HasForeignKey(pl => pl.ProductSKU);

        modelBuilder.Entity<ProductLocation>()
            .HasOne(pl => pl.Location)
            .WithOne(l => l.ProductLocation)
            .HasForeignKey<ProductLocation>(pl => pl.LocationId);
        // PRODUCT LOCATION

        // SHIPMENT
        modelBuilder.Entity<Shipment>()
            .Property(e => e.ShipmentId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Shipment>()
            .HasOne(s => s.Warehouse)
            .WithMany(w => w.Shipments)
            .HasForeignKey(s => s.WarehouseId);

        modelBuilder.Entity<Shipment>()
            .HasOne(s => s.ShippedByEmployee)
            .WithMany()
            .HasForeignKey(s => s.ShippedByEmployeeId);
        // SHIPMENT

        // SHIPMENT DETAIL
        modelBuilder.Entity<ShipmentDetail>()
            .HasOne(sd => sd.Shipment)
            .WithMany(s => s.ShipmentDetails)
            .HasForeignKey(sd => sd.ShipmentId);

        modelBuilder.Entity<ShipmentDetail>()
            .Property(e => e.ShipmentDetailId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ShipmentDetail>()
            .HasOne(sd => sd.Product)
            .WithMany(p => p.ShipmentDetails)
            .HasForeignKey(sd => sd.ProductSKU);
        // SHIPMENT DETAIL

        // BRAND
        modelBuilder.Entity<Brand>()
            .Property(b => b.BrandId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<Brand>()
            .HasOne(pt => pt.Warehouse)
            .WithMany(w => w.Brands)
            .HasForeignKey(pt => pt.WarehouseId)
            .IsRequired();
        // BRAND

        // PRODUCT TYPE
        modelBuilder.Entity<ProductType>()
            .Property(pt => pt.TypeId)
            .ValueGeneratedOnAdd();

        modelBuilder.Entity<ProductType>()
            .HasOne(pt => pt.Warehouse)
            .WithMany(w => w.ProductTypes)
            .HasForeignKey(pt => pt.WarehouseId)
            .IsRequired();
        // PRODUCT TYPE

        // TIME MAP
        modelBuilder.Entity<TimeMap>()
            .HasKey(tm => tm.TimeMapId);

        modelBuilder.Entity<TimeMap>()
            .HasOne(tm => tm.Employee)
            .WithMany()
            .HasForeignKey(tm => tm.EmployeeId)
            .IsRequired();
        // TIME MAP

        // modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        // we can customize this later 
    }

    public DbSet<Brand> Brands { get; set; }
    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<Employee> Employees { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<Orders> Orders { get; set; }
    public DbSet<ProductLocation> ProductLocations { get; set; }
    public DbSet<Shipment> Shipments { get; set; }
    public DbSet<ShipmentDetail> ShipmentDetails { get; set; }
    public DbSet<TimeMap> TimeMaps { get; set; }

}