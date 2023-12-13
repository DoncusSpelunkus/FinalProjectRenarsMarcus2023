using Application.InfraInterfaces;
using Core.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class ProductLocationRepository : IProductLocationRepository
{
    private readonly DbContextManagement _context;

    public ProductLocationRepository(DbContextManagement context)
    {
        _context = context;
    }

    public async Task<List<ProductLocation>> GetProductLocationsByWarehouseAsync(int warehouseId)
    {

        return await _context.ProductLocations
            .Where(pl => pl.WarehouseId == warehouseId)
            .Include(pl => pl.Product)  // Include the Product related entity
            .Include(pl => pl.Location) // Include the Location related entity
            .ToListAsync();
    }

    public async Task<ProductLocation> GetProductLocationAsync(string productLocationId)
    {
        return await _context.ProductLocations
            .Include(pl => pl.Product)  // Include the Product related entity
            .Include(pl => pl.Location) // Include the Location related entity
            .SingleOrDefaultAsync(pl => pl.ProductLocationId == productLocationId);
    }

    public async Task ChangeQuantity(string productLocationId, int quantityToAdd)
    {
        var productLocation = await GetProductLocationAsync(productLocationId);

        if (productLocation != null)
        {
            productLocation.Quantity += quantityToAdd;
            await _context.SaveChangesAsync();
        }
    }


    public async Task MoveQuantityAsync(string productSKU, string sourceLocationId, string destinationLocationId, int quantityToMove)
    {
        var sourceProductLocation = await _context.ProductLocations
            .FirstOrDefaultAsync(pl => pl.ProductSKU == productSKU && pl.LocationId == sourceLocationId);

        Console.WriteLine(sourceProductLocation.LocationId);
        Console.WriteLine(sourceProductLocation.Location.LocationId);
        Console.WriteLine(sourceProductLocation.Quantity);
        Console.WriteLine(sourceProductLocation.ProductLocationId);
        Console.WriteLine(sourceProductLocation.Product.SKU);
        Console.WriteLine(sourceProductLocation.WarehouseId);
        Console.WriteLine(sourceProductLocation.Warehouse.WarehouseId);

        var destinationProductLocation = await _context.ProductLocations
            .FirstOrDefaultAsync(pl => pl.ProductSKU == productSKU && pl.LocationId == destinationLocationId);

        if (sourceProductLocation != null)
        {
            sourceProductLocation.Quantity -= quantityToMove;

            if (destinationProductLocation != null)
            {
                destinationProductLocation.Quantity += quantityToMove;
            }
            else
            {
                destinationProductLocation = new ProductLocation
                {
                    ProductSKU = productSKU,
                    LocationId = destinationLocationId,
                    Quantity = quantityToMove,
                    WarehouseId = sourceProductLocation.WarehouseId, // maybe security issue 
                    LastUpdated = DateTime.UtcNow // debug here 
                };
                Console.WriteLine(destinationProductLocation.LastUpdated);
                _context.ProductLocations.Add(destinationProductLocation);
            }

            if (sourceProductLocation.Quantity == 0)
            {
                _context.ProductLocations.Remove(sourceProductLocation);
            }

            await _context.SaveChangesAsync();
        }
        else
        {
            Console.WriteLine("something went wrong with method move :(");
        }
    }

    public async Task UpdateLastUpdatedAsync(string productLocationId, DateTime lastUpdated)
    {
        var productLocation = await GetProductLocationAsync(productLocationId);

        if (productLocation != null)
        {
            productLocation.LastUpdated = lastUpdated;
            await _context.SaveChangesAsync();
        }
    }


    public async Task<ProductLocation> CreateProductLocationAsync(ProductLocation productLocation)
    {
        try
        {
           
            _context.ProductLocations.Add(productLocation);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return productLocation;
    }
}