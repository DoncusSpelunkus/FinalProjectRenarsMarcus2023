using Core.Entities;

namespace Application.InfraInterfaces;

public interface ILocationRepository
{
    Task<List<Location>> GetLocationsByWarehouseAsync(int warehouseId);
    Task<Location> CreateLocationAsync(Location location);
    Task<Location> UpdateLocationAsync(Location location);
    Task<bool> DeleteLocationAsync(string id);

    Task<List<Location>> CreateLocationBatch(List<Location> locations);

    Task<int> getBiggestAisleInt(int warehouseId);
}