using Application.Dtos;

namespace Application.IServices;

public interface IProductLocationService
{
    Task<List<ProductLocationDto>> GetProductLocationsByWarehouseAsync(int warehouseId);
    Task<ProductLocationDto> GetProductLocationAsync(int productLocationId);
    Task IncreaseQuantityAsync(int productLocationId, int quantityToAdd);
    Task DecreaseQuantityAsync(int productLocationId, int quantityToRemove);
    Task MoveQuantityAsync(string productSKU, string sourceLocationId, string destinationLocationId, int quantityToMove);
    Task UpdateLastUpdatedAsync(int productLocationId, DateTime lastUpdated);
    Task<ProductLocationDto> CreateProductLocationAsync(CreateProductLocationDto createProductLocationDto);
    
    // New methods from ILogRepository
    Task<List<LogDto>> GetLogsByWarehouseAsync(int warehouseId);
    Task<LogDto> CreateLogAsync(LogDto createLogDto);
    Task<bool> DeleteLogsOlderThanOneYearAsync();
}