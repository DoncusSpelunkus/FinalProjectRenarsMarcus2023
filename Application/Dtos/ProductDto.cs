namespace Application.Dtos;

public class ProductDto
{
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
}