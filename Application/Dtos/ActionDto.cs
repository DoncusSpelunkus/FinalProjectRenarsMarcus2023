using Application.Helpers;

namespace Application.Dtos
{
    public class ActionDto // We use this dto for both Admin changes and Employee changes
    {
        public int EmployeeId { get; set; }
        public string? LocationId { get; set; }
        public string? ProductSKU { get; set; }
        public string? SourcePLocationId { get; set; }
        public string? DestinationPLocationId { get; set; }
        public int Quantity { get; set; }
        public int WarehouseId { get; set; }
        public ActionEnum Type { get; set; } // Fix: Make RoleEnum public
    }
}