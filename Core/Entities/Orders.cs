using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Orders
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // If you want the ID to be auto-generated
    public int OrderId { get; set; }
    public string OrderNumber { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDeliveryDate { get; set; }
    public string ProductSKU { get; set; }
    public int Quantity { get; set; }
    public double TotalAmount { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
}