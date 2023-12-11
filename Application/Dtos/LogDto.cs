namespace Application.Dtos;

public class LogDto
{
    public int LogId { get; set; }
    public string ProductSKU { get; set; }
    public string FromLocationId { get; set; }
    public string ToLocationId { get; set; }
    public int Quantity { get; set; }
    public int UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public int WarehouseId { get; set; }
}