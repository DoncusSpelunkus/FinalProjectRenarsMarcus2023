using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.Property(l => l.LocationId).IsRequired();
        builder.Property(l => l.Aisle);
        builder.Property(l => l.Rack);
        builder.Property(l => l.Shelf);
        builder.Property(l => l.Bin);
        
        builder.Property(l => l.WarehouseId).IsRequired();
    }
}