namespace Application.Dtos;

public class MoveLogDto
{
    public int LogId { get; set; }
    public string ProductSKU { get; set; }
    public string FromLocationId { get; set; }
    public string ToLocationId { get; set; }
    public int Quantity { get; set; }
    public int UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public int WarehouseId { get; set; }

    public string? Type { get; set; }
}