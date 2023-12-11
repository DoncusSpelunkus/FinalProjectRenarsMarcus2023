using Application.InfraInterfaces;
using Core.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class TypeRepository : ITypeRepository
{
    private readonly DbContextManagement _context;

    public TypeRepository(DbContextManagement context)
    {
        _context = context;
    }

    public async Task<List<ProductType>> GetTypesByWarehouseAsync(int warehouseId)
    {
        return await _context.ProductTypes
            .Where(pt => pt.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<ProductType> CreateProductTypeAsync(ProductType type)
    {
        _context.ProductTypes.Add(type);
        await _context.SaveChangesAsync();
        return type;
    }

    public async Task<bool> DeleteProductTypeAsync(int id)
    {
        var type = await GetTypeByIdAsync(id);
        if (type == null)
        {
            return false; 
        }
        _context.ProductTypes.Remove(type);
        await _context.SaveChangesAsync();
        return true; 
    }

    public async Task<ProductType> GetTypeByIdAsync(int id)
    {
        return await _context.ProductTypes.FirstOrDefaultAsync(p => p.TypeId == id);
    }
}