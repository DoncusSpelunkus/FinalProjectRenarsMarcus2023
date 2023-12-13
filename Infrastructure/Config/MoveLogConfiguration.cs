using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class LogConfiguration : IEntityTypeConfiguration<MoveLog>
{
    public void Configure(EntityTypeBuilder<MoveLog> builder)
    {
        builder.Property(l => l.ProductSKU).IsRequired();
        builder.Property(l => l.FromLocationId).IsRequired();
        builder.Property(l => l.ToLocationId).IsRequired();
        builder.Property(l => l.Quantity);
        builder.Property(l => l.UserId);
        builder.Property(l => l.Timestamp);
        builder.Property(l => l.WarehouseId).IsRequired();
        
    }
}