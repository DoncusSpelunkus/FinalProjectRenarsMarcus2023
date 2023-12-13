using Application.Dtos;

namespace Application.IServices;

public interface IProductLocationService
{
    Task<List<ProductLocationDto>> GetProductLocationsByWarehouseAsync(int warehouseId);
    Task<ProductLocationDto> GetProductLocationAsync(string productLocationId);
    Task IncreaseQuantityAsync(ChangeProductDto changeProductDto);
    Task DecreaseQuantityAsync(ChangeProductDto changeProductDto);
    Task MoveQuantityAsync(ChangeProductDto changeProductDto);
    Task UpdateLastUpdatedAsync(ChangeProductDto changeProductDto, DateTime lastUpdated);
    Task<ProductLocationDto> CreateProductLocationAsync(CreateProductLocationDto createProductLocationDto);
    Task<List<MoveLogDto>> GetLogsByWarehouseAsync(int warehouseId);
    Task<bool> DeleteLogsOlderThanOneYearAsync();
    Task<List<MoveLogDto>> GetAdminLogsByWarehouseAsync(int warehouseId);
}