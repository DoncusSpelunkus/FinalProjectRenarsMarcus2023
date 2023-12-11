using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.SKU).IsRequired();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description);
        builder.Property(p => p.Category);
        builder.Property(p => p.Weight);
        builder.Property(p => p.Length);
        builder.Property(p => p.Width);
        builder.Property(p => p.Height);
        builder.Property(p => p.SupplierName);
        builder.Property(p => p.SupplierContact);
        builder.Property(p => p.ExpireDateTime);
        builder.Property(p => p.MinimumCapacity);
        builder.Property(p => p.BrandId).IsRequired();
        builder.Property(p => p.TypeId).IsRequired();
        builder.Property(p => p.WarehouseId).IsRequired();
    }
}