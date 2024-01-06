using Core.Entities;

namespace Application.InfraInterfaces;

    public interface IProductLocationRepository
    {
        Task<List<ProductLocation>> GetProductLocationsByWarehouseAsync(int warehouseId);
        Task<ProductLocation> GetProductLocationAsync(string productLocationId);
        Task ChangeQuantity(string productLocationId, int modifiedQuantity);
        Task MoveToExistingLocationAsync(string sourceLocationId, string destinationLocationId, int quantityToMove);
        Task MoveToNewLocationAsync(ProductLocation productLocation, string source);
        Task UpdateLastUpdatedAsync(string productLocationId, DateTime lastUpdated);
        Task<ProductLocation> CreateProductLocationAsync(ProductLocation productLocation);

    }