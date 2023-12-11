using Core.Entities;

namespace Application.Dtos;

public class ProductLocationDto
{
    public int ProductLocationId { get; set; }
    public ProductDto Product { get; set; }
    public LocationDto Location { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }
    public int WarehouseId { get; set; }
}