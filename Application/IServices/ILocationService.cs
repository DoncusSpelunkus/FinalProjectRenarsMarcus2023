using Application.Dtos;

namespace Application.IServices;

public interface ILocationService
{
    Task<LocationDto> GetLocationAsync(LocationDto locationDto);
    Task<List<LocationDto>> GetLocationsByWarehouseAsync(int warehouseId);
    Task<LocationDto> CreateLocationAsync(LocationDto locationDto);
    Task<LocationDto> UpdateLocationAsync(LocationDto locationDto);
    Task<bool> DeleteLocationAsync(LocationDto locationDto);

    Task<List<LocationDto>> CreateLocationBatch(LocationDto locationDto);
}