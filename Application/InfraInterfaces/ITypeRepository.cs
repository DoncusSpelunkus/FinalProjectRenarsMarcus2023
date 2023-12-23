using Core.Entities;

namespace Application.InfraInterfaces;

public interface ITypeRepository
{
    Task<List<ProductType>> GetTypesByWarehouseAsync(int warehouseId);
    Task<ProductType> CreateProductTypeAsync(ProductType type);

    Task<ProductType> UpdateProductTypeAsync(ProductType type);
    Task<bool> DeleteProductTypeAsync(int id);
    Task<ProductType> GetTypeByIdAsync(int id);

}