using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class AdminLogConfiguration : IEntityTypeConfiguration<AdminLog>
{
    public void Configure(EntityTypeBuilder<AdminLog> builder)
    {
        builder.Property(l => l.ProductlocationId).IsRequired();
        builder.Property(l => l.QuantityChange).IsRequired();
        builder.Property(l => l.EmployeeId).IsRequired();
        builder.Property(l => l.Timestamp);
        builder.Property(l => l.WarehouseId).IsRequired();
        
    }
}