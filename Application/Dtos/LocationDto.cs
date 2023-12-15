namespace Application.Dtos;

public class LocationDto 
// LocationDto can be used in two ways, as a template for creation of batches of new locations
// or to create singular locations.
{
    public string? LocationId { get; set; }
    public int Aisle { get; set; }
    public int Rack { get; set; }
    public int Shelf { get; set; }
    public int Bin { get; set; }
    public int WarehouseId { get; set; }
}