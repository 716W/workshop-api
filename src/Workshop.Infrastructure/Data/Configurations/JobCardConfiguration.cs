using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Data.Configurations;

public sealed class JobCardConfiguration : IEntityTypeConfiguration<JobCard>
{
    public void Configure(EntityTypeBuilder<JobCard> builder)
    {
        builder.HasKey(j => j.Id);

        builder.Property(j => j.JobNumber).IsRequired().HasMaxLength(30);
        builder.Property(j => j.Description).HasMaxLength(500);
        builder.Property(j => j.InspectionNotes).HasMaxLength(2000);
        builder.Property(j => j.EstimatedCost).HasPrecision(18, 2);
        builder.Property(j => j.FinalCost).HasPrecision(18, 2);
        builder.Property(j => j.Status).HasConversion<string>().HasMaxLength(30);

        builder.HasIndex(j => j.JobNumber).IsUnique();

        builder.HasOne(j => j.Mechanic)
               .WithMany(m => m.JobCards)
               .HasForeignKey(j => j.MechanicId)
               .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(j => j.Invoice)
               .WithOne(i => i.JobCard)
               .HasForeignKey<Invoice>(i => i.JobCardId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
