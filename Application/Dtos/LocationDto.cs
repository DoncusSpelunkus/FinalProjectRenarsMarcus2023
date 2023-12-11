namespace Application.Dtos;

public class LocationDto
{
    public string LocationId { get; set; }
    public string Aisle { get; set; }
    public string Rack { get; set; }
    public string Shelf { get; set; }
    public string Bin { get; set; }
    public int WarehouseId { get; set; }
}