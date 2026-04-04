using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the <see cref="QuotationItem"/> entity.
/// </summary>
public sealed class QuotationItemConfiguration : IEntityTypeConfiguration<QuotationItem>
{
    public void Configure(EntityTypeBuilder<QuotationItem> builder)
    {
        builder.ToTable("QuotationItems");

        builder.HasKey(i => i.Id);

        // ── Type ───────────────────────────────────────────────────────────────
        builder.Property(i => i.Type)
               .HasConversion<string>()
               .HasMaxLength(10)
               .IsRequired();

        // ── Description ────────────────────────────────────────────────────────
        builder.Property(i => i.Description)
               .HasMaxLength(500)
               .IsRequired();

        // ── Quantity ───────────────────────────────────────────────────────────
        builder.Property(i => i.Quantity)
               .IsRequired();

        // ── Money ──────────────────────────────────────────────────────────────
        builder.Property(i => i.UnitPrice)
               .HasPrecision(18, 2)
               .IsRequired();

        builder.Property(i => i.TotalPrice)
               .HasPrecision(18, 2)
               .IsRequired();
    }
}
