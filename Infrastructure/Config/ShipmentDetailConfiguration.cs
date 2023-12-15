using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class ShipmentDetailConfiguration : IEntityTypeConfiguration<ShipmentDetail>
{
    public void Configure(EntityTypeBuilder<ShipmentDetail> builder)
    {
        builder.Property(sd => sd.ShipmentDetailId).IsRequired();
        builder.Property(sd => sd.ShipmentId).IsRequired();
        builder.Property(sd => sd.ProductSKU).IsRequired();
        builder.Property(sd => sd.Quantity).IsRequired();
        
    }
}