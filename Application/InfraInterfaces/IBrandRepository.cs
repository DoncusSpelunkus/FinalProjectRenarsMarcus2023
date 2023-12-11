using Core.Entities;

namespace Application.InfraInterfaces;

public interface IBrandRepository
{
    Task<List<Brand>> GetBrandsByWarehouseAsync(int warehouseId);
    Task<Brand> CreateBrandAsync(Brand brand);
    Task<bool> DeleteBrandAsync(int id);
    Task<Brand> GetBrnadByIdAsync(int sku);
}