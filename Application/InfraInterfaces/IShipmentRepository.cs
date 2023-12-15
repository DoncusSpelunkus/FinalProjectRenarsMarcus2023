using Core.Entities;

namespace Application.InfraInterfaces;

public interface IShipmentRepository
{
    Task<Shipment> CreateShipmentAsync(Shipment shipment);
    Task<bool> DeleteShipmentAsync(int shipmentId);
    Task<ShipmentDetail> AddProductToShipmentAsync(int shipmentId, ShipmentDetail shipmentDetail);
    Task<bool> RemoveProductFromShipmentAsync(int shipmentId, int shipmentDetailId);
    Task<bool> ChangeProductQuantityInShipmentAsync(int shipmentId, int shipmentDetailId, int newQuantity);
    Task<List<Shipment>> GetShipmentsByWarehouseAsync(int warehouseId);
    Task<Shipment> GetShipmentByIdAsync(int shipmentId);
}