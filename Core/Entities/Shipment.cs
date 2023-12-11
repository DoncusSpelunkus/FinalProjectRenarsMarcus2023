using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Shipment
{
    [Key]
    public int ShipmentId { get; set; }
    public int WarehouseId { get; set; }
    public int ShippedByEmployeeId { get; set; }
    public DateTime DateShipped { get; set; }
    public Warehouse Warehouse { get; set; }
    public Employee ShippedByEmployee { get; set; }
    
    // bug 
    public List<ShipmentDetail> ShipmentDetails { get; set; } // swap to list  from IEnumerable doesn't contain add or remove 
}