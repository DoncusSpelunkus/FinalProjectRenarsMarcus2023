using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Location
{
    [Key]
    public string LocationId { get; set; }
    public int Aisle { get; set; }
    public int Rack { get; set; }
    public int Shelf { get; set; }
    public int Bin { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
}