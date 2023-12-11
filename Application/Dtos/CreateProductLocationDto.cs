namespace Application.Dtos;

public class CreateProductLocationDto
{
    public string ProductSKU { get; set; }
    public string LocationId { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }
    public int WarehouseId { get; set; }
}