using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class ShipmentDetail
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // If you want the ID to be auto-generated
    public int? ShipmentDetailId { get; set; }
    public int? ShipmentId { get; set; }
    public string ProductSKU { get; set; }
    public int Quantity { get; set; }
    public Shipment Shipment { get; set; }
    public Product Product { get; set; }
}