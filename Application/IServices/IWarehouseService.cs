using Application.Dtos;

namespace Application.IServices;
public interface IWarehouseService
{
    Task<WarehouseDto> CreateWarehouse(WarehouseDto warehouseDto); 
    Task<WarehouseDto> UpdateWarehouseAsync(int warehouseId, WarehouseDto warehouseDto);
    Task<bool> DeleteWarehouseAsync(int warehouseId);
}