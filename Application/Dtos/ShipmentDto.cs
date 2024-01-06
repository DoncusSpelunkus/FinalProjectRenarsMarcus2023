namespace Application.Dtos;

public class ShipmentDto
{
    public int? ShipmentId { get; set; }
    public int WarehouseId { get; set; }
    public int? ShippedByEmployeeId { get; set; }
    public DateTime DateShipped { get; set; } 
    public List<ShipmentDetailDto> ShipmentDetails { get; set; }
}