using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class ProductLocation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // If you want the ID to be auto-generated
    public int ProductLocationId { get; set; }
    public string ProductSKU { get; set; }
    public string LocationId { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }
    public int WarehouseId { get; set; }
    public Product Product { get; set; }
    public Location Location { get; set; }
    public Warehouse Warehouse { get; set; }
}