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
            .ToListAsync() ?? throw new ApplicationException("Source location does not exist");
    }


    public async Task ChangeQuantity(string productLocationId, int modifiedQuantity)
    {


        var productLocation = await _context.ProductLocations
         .Where(pl => pl.ProductLocationId == productLocationId)
         .Include(pl => pl.Product)
         .Include(pl => pl.Location)
         .FirstOrDefaultAsync() ?? throw new ApplicationException("Source location does not exist");

        productLocation.Quantity = modifiedQuantity;

        _context.Entry(productLocation).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            var innerException = ex.InnerException;
        }

    }


    public async Task MoveToExistingLocationAsync(string sourceLocationId, string destinationLocationId, int quantityToMove)
    {
        var sourceProductLocation = await _context.ProductLocations
            .FirstOrDefaultAsync(pl => pl.ProductLocationId == sourceLocationId) ?? throw new ApplicationException("Source location does not exist");

        var destinationProductLocation = await _context.ProductLocations
            .FirstOrDefaultAsync(pl => pl.ProductLocationId == destinationLocationId) ?? throw new ApplicationException("Destination location does not exist");


        if (sourceProductLocation.Quantity - quantityToMove < 0)
        {
            throw new ApplicationException("Not enough quantity to move, try moving less");
        }

        sourceProductLocation.Quantity -= quantityToMove;

        destinationProductLocation.Quantity += quantityToMove;

        if (sourceProductLocation.Quantity == 0)
        {
            _context.ProductLocations.Remove(sourceProductLocation);
        }

        await _context.SaveChangesAsync();
    }

    public async Task MoveToNewLocationAsync(ProductLocation productLocation, string source)
    {
        var sourceProductLocation = await _context.ProductLocations
            .FirstOrDefaultAsync(pl => pl.ProductLocationId == source) ?? throw new ApplicationException("Source location does not exist");

        if (sourceProductLocation.Quantity - productLocation.Quantity < 0)
        {
            throw new ApplicationException("Not enough quantity to move, try moving less");
        }

        productLocation.ProductSKU = sourceProductLocation.ProductSKU;

        var destinationProductLocation = await CreateProductLocationAsync(productLocation) ?? throw new ApplicationException("Destination location does not exist");

        sourceProductLocation.Quantity -= productLocation.Quantity;

        if (sourceProductLocation.Quantity == 0)
        {
            _context.ProductLocations.Remove(sourceProductLocation);
        }

        await _context.SaveChangesAsync();
    }

    public async Task UpdateLastUpdatedAsync(string productLocationId, DateTime lastUpdated)
    {
        var productLocation = await _context.ProductLocations.FindAsync(productLocationId) ?? throw new ApplicationException("Source location does not exist");

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
            _ = await _context.Locations
            .Where(l => l.LocationId == productLocation.LocationId).FirstAsync();
        }
        catch (Exception e)
        {
            throw new ApplicationException("Location does not exist");
        }

        var Something = await _context.ProductLocations
        .Where(pl => pl.LocationId == productLocation.LocationId).FirstOrDefaultAsync();

        if (Something != null)
        {
            throw new ApplicationException("Location already has a product location");
        }

        try
        {
            _ = await _context.Products
            .Where(p => p.SKU == productLocation.ProductSKU).FirstAsync();
        }
        catch (Exception e)
        {
            throw new ApplicationException("Product does not exist");
        }

        _context.ProductLocations.Add(productLocation);
        await _context.SaveChangesAsync();
        return productLocation;
    }

    public async Task<ProductLocation> GetProductLocationAsync(string productLocationId)
    {
        return await _context.ProductLocations.FindAsync(productLocationId) ?? throw new ApplicationException("Source location does not exist");
    }
}