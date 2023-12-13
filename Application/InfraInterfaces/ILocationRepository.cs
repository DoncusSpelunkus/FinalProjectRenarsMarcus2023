using Core.Entities;

namespace Application.InfraInterfaces;

public interface ILocationRepository
{
    Task<Location> GetLocationAsync(Location location);
    Task<List<Location>> GetLocationsByWarehouseAsync(int warehouseId);
    Task<Location> CreateLocationAsync(Location location);
    Task<Location> UpdateLocationAsync(Location location);
    Task<bool> DeleteLocationAsync(Location location);

    Task<List<Location>> CreateLocationBatch(List<Location> locations);

    Task<int> getBiggestAisleInt(int warehouseId);
}