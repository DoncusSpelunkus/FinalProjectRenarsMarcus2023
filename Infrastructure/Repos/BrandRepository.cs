using Application.InfraInterfaces;
using Core.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class BrandRepository : IBrandRepository
{
    private readonly DbContextManagement _context;

    public BrandRepository(DbContextManagement context)
    {
        _context = context;
    }

    public async Task<List<Brand>> GetBrandsByWarehouseAsync(int warehouseId)
    {
        return await _context.Brands
            .Where(pt => pt.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<Brand> CreateBrandAsync(Brand brand)
    {
        _context.Brands.Add(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task<bool> DeleteBrandAsync(int id)
    {
        var brand = await GetBrnadByIdAsync(id);
        if (brand == null)
        {
            return false; 
        }
        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        return true;
        
    }

    public async Task<Brand> GetBrnadByIdAsync(int id)
    {
        return await _context.Brands.FirstOrDefaultAsync(p => p.BrandId == id);
    }

    public async Task<Brand> UpdateBrandAsync(Brand brand)
    {
        var existingBrand = await _context.Brands.FindAsync(brand.BrandId);

        if (existingBrand == null)
        {
            throw new ApplicationException("Employee not found");
        }

        
        _context.Entry(existingBrand).CurrentValues.SetValues(brand);

        await _context.SaveChangesAsync();

        return existingBrand;

    }
}