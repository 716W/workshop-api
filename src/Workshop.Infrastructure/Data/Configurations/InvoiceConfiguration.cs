using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Data.Configurations;

public sealed class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(50);
        builder.Property(i => i.PartsCost).HasPrecision(18, 2);
        builder.Property(i => i.LaborCost).HasPrecision(18, 2);
        builder.Property(i => i.TaxAmount).HasPrecision(18, 2);
        builder.Property(i => i.TotalAmount).HasPrecision(18, 2);

        builder.HasIndex(i => i.InvoiceNumber).IsUnique();

        // ── Relationships ──────────────────────────────────────────────────────

        // Optional FK → JobCards (job-completion invoices).
        builder.HasOne(i => i.JobCard)
               .WithMany()
               .HasForeignKey(i => i.JobCardId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);

        // Optional FK → ServiceRequests (inspection-fee invoices raised on rejection).
        builder.HasOne(i => i.ServiceRequest)
               .WithMany()
               .HasForeignKey(i => i.ServiceRequestId)
               .IsRequired(false)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
