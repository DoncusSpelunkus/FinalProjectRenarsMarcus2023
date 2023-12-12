using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Warehouse
{
    [Key]
    public int WarehouseId { get; set; }
    public string Name { get; set; }
    public string Address { get; set; }
    public int Capacity { get; set; }
    public IEnumerable<Employee> Employees { get; set; }
    public IEnumerable<Location> Locations { get; set; }
    public IEnumerable<Product> Products { get; set; }
    public IEnumerable<Orders> Orders { get; set; }
    public IEnumerable<Shipment> Shipments { get; set; }
    public IEnumerable<ProductLocation> ProductLocations { get; set; }
    public IEnumerable<ProductType> ProductTypes { get; set; }
    
    public IEnumerable<Brand> Brands { get; set; }

    public IEnumerable<AdminLog> AdminLogs { get; set; }

    public IEnumerable<MoveLog> MoveLogs { get; set; }
}