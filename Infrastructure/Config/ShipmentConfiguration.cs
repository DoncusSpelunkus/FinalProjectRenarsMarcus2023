using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
{
    public void Configure(EntityTypeBuilder<Shipment> builder)
    {
        builder.Property(s => s.WarehouseId).IsRequired();
        builder.Property(s => s.ShippedByEmployeeId);
        builder.Property(s => s.DateShipped);
        
    }
}