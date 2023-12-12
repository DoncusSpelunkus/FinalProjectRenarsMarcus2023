using Core.Entities;

namespace Application.Dtos;

public class ProductLocationDto
{
    public string? ProductLocationId { get; set; }
    public string ProductSku { get; set; }
    public string LocationId { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }
    public int WarehouseId { get; set; }
}