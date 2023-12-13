using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class AdminLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // If you want the ID to be auto-generated
    public int AdminLogId { get; set; }
    public string ProductlocationId { get; set; }
    public string ProductSKU { get; set; }
    public int QuantityChange { get; set; }
    public DateTime Timestamp { get; set; }
    public int WarehouseId { get; set; } 
    public int EmployeeId { get; set; }
    public Warehouse Warehouse { get; set; }
    public Employee Employee { get; set; }
    public Product Product { get; set; }
  
}