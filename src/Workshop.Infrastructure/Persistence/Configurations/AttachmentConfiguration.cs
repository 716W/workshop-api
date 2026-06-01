using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Persistence.Configurations;

public sealed class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachments");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.FilePath)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.UploadedBy)
            .HasMaxLength(450); // Matches ASP.NET Identity user ID length

        // Audit fields from BaseAuditableEntity
        builder.Property(a => a.CreatedBy)
            .HasMaxLength(450);

        builder.Property(a => a.UpdatedBy)
            .HasMaxLength(450);

        // ── Relationship ──────────────────────────────────────────────────────
        builder.HasOne(a => a.ServiceRequest)
            .WithMany()
            .HasForeignKey(a => a.ServiceRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
