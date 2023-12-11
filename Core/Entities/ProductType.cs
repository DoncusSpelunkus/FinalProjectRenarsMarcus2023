using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class ProductType
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // If you want the ID to be auto-generated
    public int TypeId { get; set; }
    public string Name { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } 
    public IEnumerable<Product> Products { get; set; }
}