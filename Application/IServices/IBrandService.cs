using Application.Dtos;

namespace Application.IServices;

public interface IBrandService
{
    Task<List<BrandDto>> GetBrandsByWarehouseAsync(int warehouseId);
    Task<BrandDto> CreateBrandAsync(BrandDto brandDto);
    Task<bool> DeleteBrandAsync(int id);
    Task<BrandDto> GetBrandByIdAsync(int id);
}