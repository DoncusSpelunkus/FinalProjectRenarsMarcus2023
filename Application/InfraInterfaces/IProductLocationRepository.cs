﻿using Core.Entities;

namespace Application.InfraInterfaces;

    public interface IProductLocationRepository
    {
        Task<List<ProductLocation>> GetProductLocationsByWarehouseAsync(int warehouseId);
        Task<ProductLocation> GetProductLocationAsync(string productLocationId);
        Task ChangeQuantity(string productLocationId, int quantityToAdd);
        Task MoveToExistingLocationAsync(string sourceLocationId, string destinationLocationId, int quantityToMove);
        Task MoveToNewLocationAsync(ProductLocation productLocation);
        Task UpdateLastUpdatedAsync(string productLocationId, DateTime lastUpdated);
        Task<ProductLocation> CreateProductLocationAsync(ProductLocation productLocation);

    }
    // Task<List<ProductLocation>> GetProductLocationsByWarehouseAsync(int warehouseId);
    // Task<ProductLocation> GetProductLocationAsync(int productLocationId);
    // Task IncreaseQuantityAsync(int productLocationId, int quantityToAdd);
    // Task DecreaseQuantityAsync(int productLocationId, int quantityToRemove);
    // Task MoveQuantityAsync(int sourceProductLocationId, int destinationProductLocationId, int quantityToMove);
    // Task UpdateLastUpdatedAsync(int productLocationId, DateTime lastUpdated);