using Core.Entities;

namespace Application.InfraInterfaces;

public interface IProductRepository
{
    Task<Product> GetProductBySKUAsync(string sku);
    Task<List<Product>> GetProductsByWarehouseAsync(int warehouseId);
    Task<Product> CreateProductAsync(Product product);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(string sku);
}