namespace Application.Dtos;

public class ShipmentDetailDto
{
    public int? ShipmentDetailId { get; set; }
    public int ShipmentId { get; set; }
    public string ProductSKU { get; set; }
    public int Quantity { get; set; }
}