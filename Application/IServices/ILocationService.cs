using Application.Dtos;

namespace Application.IServices;

public interface ILocationService
{
    Task<List<LocationDto>> GetLocationsByWarehouseAsync(int warehouseId);
    Task<LocationDto> CreateLocationAsync(LocationDto locationDto);
    Task<LocationDto> UpdateLocationAsync(LocationDto locationDto);
    Task<bool> DeleteLocationAsync(string id);
    Task<List<LocationDto>> CreateLocationBatch(LocationDto locationDto);
}