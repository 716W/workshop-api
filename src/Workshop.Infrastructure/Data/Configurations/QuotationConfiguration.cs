using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the <see cref="Quotation"/> entity.
/// </summary>
public sealed class QuotationConfiguration : IEntityTypeConfiguration<Quotation>
{
    public void Configure(EntityTypeBuilder<Quotation> builder)
    {
        builder.ToTable("Quotations");

        builder.HasKey(q => q.Id);

        // ── Money ──────────────────────────────────────────────────────────────
        builder.Property(q => q.GrandTotal)
               .HasPrecision(18, 2)
               .IsRequired();

        // ── Notes ─────────────────────────────────────────────────────────────
        builder.Property(q => q.Notes)
               .HasMaxLength(2000);

        // ── Items (1:N) ────────────────────────────────────────────────────────
        builder.HasMany(q => q.Items)
               .WithOne(i => i.Quotation)
               .HasForeignKey(i => i.QuotationId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
