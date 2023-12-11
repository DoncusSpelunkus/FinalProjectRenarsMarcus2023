using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Config;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.Property(e => e.Username).IsRequired();
        builder.Property(e => e.PasswordHash).IsRequired();
        builder.Property(e => e.PasswordSalt).IsRequired();
        builder.Property(e => e.Name).IsRequired();
        builder.Property(e => e.Email).IsRequired();
        builder.Property(e => e.Role).IsRequired();
        builder.Property(e => e.WarehouseId).IsRequired();
    }
}