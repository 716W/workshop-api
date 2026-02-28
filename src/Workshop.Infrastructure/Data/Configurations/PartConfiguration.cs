using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Data.Configurations;

public sealed class PartConfiguration : IEntityTypeConfiguration<Part>
{
    public void Configure(EntityTypeBuilder<Part> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name).IsRequired().HasMaxLength(150);
        builder.Property(p => p.PartNumber).IsRequired().HasMaxLength(50);
        builder.Property(p => p.UnitPrice).HasPrecision(18, 2);

        builder.HasIndex(p => p.PartNumber).IsUnique();
    }
}
