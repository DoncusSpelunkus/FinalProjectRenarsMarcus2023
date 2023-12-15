using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class LogConfiguration : IEntityTypeConfiguration<Log>
{
    public void Configure(EntityTypeBuilder<Log> builder)
    {
        builder.Property(l => l.LogId).IsRequired();
        builder.Property(l => l.ProductSKU);
        builder.Property(l => l.FromLocationId);
        builder.Property(l => l.ToLocationId);
        builder.Property(l => l.Quantity);
        builder.Property(l => l.UserId);
        builder.Property(l => l.Timestamp);
        builder.Property(l => l.WarehouseId).IsRequired();
    }
}