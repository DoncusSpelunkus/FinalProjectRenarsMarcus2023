using Application.Dtos;

namespace Application.IServices;

public interface IProductService
{
    Task<ProductDto> GetProductBySKUAsync(string sku);
    Task<List<ProductDto>> GetProductsByWarehouseAsync(int warehouseId);
    Task<ProductDto> CreateProductAsync(ProductDto productDto);
    Task<ProductDto> UpdateProductAsync(string sku, ProductDto productDto);
    Task<bool> DeleteProductAsync(string sku);
}