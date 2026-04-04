using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Data.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the <see cref="PurchaseNeed"/> entity.
/// </summary>
public sealed class PurchaseNeedConfiguration : IEntityTypeConfiguration<PurchaseNeed>
{
    public void Configure(EntityTypeBuilder<PurchaseNeed> builder)
    {
        builder.ToTable("PurchaseNeeds");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.PartName)
               .IsRequired()
               .HasMaxLength(200);

        builder.Property(p => p.Quantity)
               .IsRequired();

        builder.Property(p => p.DateRequested)
               .IsRequired();

        // ── Relationships ──────────────────────────────────────────────────────

        // Optional FK → ServiceRequests (no cascade delete — preserve purchase history).
        builder.HasOne<ServiceRequest>()
               .WithMany()
               .HasForeignKey(p => p.ServiceRequestId)
               .OnDelete(DeleteBehavior.Restrict);

        // Optional FK → JobCards.
        builder.HasOne<JobCard>()
               .WithMany()
               .HasForeignKey(p => p.JobCardId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.SetNull);
    }
}
