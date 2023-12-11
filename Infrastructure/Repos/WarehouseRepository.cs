using Application.InfraInterfaces;
using Core.Entities;
using Infrastructure.Contexts;

namespace Infrastructure.Repos;

public class WarehouseRepository : IWarehouseRepository
{
    private DbContextManagement _context;

    public WarehouseRepository(DbContextManagement context)
    {
        _context = context;
    }

    public async Task<Warehouse> CreateWarehouseAsync(Warehouse warehouse)
    {
        
        _context.Warehouses.Add(warehouse);
        await _context.SaveChangesAsync();

        Console.WriteLine(warehouse);

        return warehouse;
    }

    public async Task<Warehouse> UpdateWarehouseAsync(Warehouse warehouse)
    {
        var existingWarehouse = await _context.Warehouses.FindAsync(warehouse.WarehouseId);

        if (existingWarehouse != null)
        {
            existingWarehouse.Name = warehouse.Name;
            existingWarehouse.Address = warehouse.Address;
            existingWarehouse.Capacity = warehouse.Capacity;
           
            await _context.SaveChangesAsync();
            return existingWarehouse;
        }
        // just in case the warehouse not found 
        return null; 
    }

    public async Task<bool> DeleteWarehouseAsync(int warehouseId)
    {
        var warehouse = await _context.Warehouses.FindAsync(warehouseId);

        if (warehouse != null)
        {
            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();
            return true;
        }

        return false; 
    }

    public async Task<Warehouse> GetWarehouseByIdAsync(int warehouseId)
    {
        return await _context.Warehouses.FindAsync(warehouseId);
    }
}