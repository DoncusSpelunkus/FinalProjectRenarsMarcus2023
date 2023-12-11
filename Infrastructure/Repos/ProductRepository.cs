using Application.InfraInterfaces;
using Core.Entities;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repos;

public class ProductRepository : IProductRepository
{
    private readonly DbContextManagement _context;

    public ProductRepository(DbContextManagement context)
    {
        _context = context;
    }
    
    public async Task<Product> GetProductBySKUAsync(string sku)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.SKU == sku);
    }

    public async Task<List<Product>> GetProductsByWarehouseAsync(int warehouseId)
    {
        return await _context.Products
            .Where(p => p.WarehouseId == warehouseId)
            .ToListAsync();
    }

    public async Task<Product> CreateProductAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product; 
    }

    public async Task<bool> DeleteProductAsync(string sku)
    {
        var product = await GetProductBySKUAsync(sku);
        if (product == null)
        {
            return false; 
        }
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true; 
    }
}