using Application.Dtos;

namespace Application.IServices;

public interface IProductLocationService
{
    Task<List<ProductLocationDto>> GetProductLocationsByWarehouseAsync(int warehouseId);
    Task<ProductLocationDto> GetProductLocationAsync(string productLocationId);
    Task ChangeQuantity(ActionDto actionDto);
    Task MoveQuantityAsync(ActionDto actionDto);
    Task UpdateLastUpdatedAsync(ActionDto actionDto, DateTime lastUpdated);
    Task<ProductLocationDto> CreateProductLocationAsync(ActionDto actionDto);
    Task<List<MoveLogDto>> GetLogsByWarehouseAsync(int warehouseId);
    Task<bool> DeleteLogsOlderThanOneYearAsync();
}