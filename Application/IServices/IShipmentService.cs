using Application.Dtos;

namespace Application.IServices;

// note this class contain the logic of both shipment and shipment-details class 
public interface IShipmentService
{
    Task<ShipmentDto> CreateShipmentAsync(ShipmentDto shipmentDto);
    Task<bool> DeleteShipmentAsync(int shipmentId);
    Task<ShipmentDto> AddProductToShipmentAsync(AddToShipmentDetails shipmentDetailDto, int shipmentId);
    Task<bool> RemoveProductFromShipmentAsync(int shipmentId, int shipmentDetailId);
    Task<bool> ChangeProductQuantityInShipmentAsync( int shipmentId ,int shipmentDetailId, int newQuantity);
    Task<List<ShipmentDto>> GetShipmentsByWarehouseAsync(int warehouseId);
    Task<ShipmentDto> GetShipmentByIdAsync(int shipmentId);
}