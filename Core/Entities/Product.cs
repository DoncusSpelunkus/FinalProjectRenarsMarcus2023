using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Product
{
    [Key]
    public string SKU { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public double Weight { get; set; }
    public double Length { get; set; }
    public double Width { get; set; }
    public double Height { get; set; }
    public string SupplierName { get; set; }
    public string SupplierContact { get; set; }
    public DateTime ExpireDateTime { get; set; }
    public int MinimumCapacity  { get; set; }
    
    public int BrandId { get; set; }
    public int TypeId { get; set; }
    public int WarehouseId { get; set; }
    public Brand Brand { get; set; }
    public ProductType ProductType { get; set; }
    public Warehouse Warehouse { get; set; }
    public IEnumerable<MoveLog> MoveLogs { get; set; }
    public IEnumerable<ProductLocation> ProductLocations { get; set; }
    public IEnumerable<ShipmentDetail> ShipmentDetails { get; set; }

    public IEnumerable<AdminLog> AdminLogs { get; set; }
}