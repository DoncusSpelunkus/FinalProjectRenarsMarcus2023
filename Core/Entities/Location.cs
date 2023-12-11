using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Location
{
    [Key]
    public string LocationId { get; set; }
    public string Aisle { get; set; }
    public string Rack { get; set; }
    public string Shelf { get; set; }
    public string Bin { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
}