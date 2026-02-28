using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Data.Configurations;

public sealed class JobCardPartConfiguration : IEntityTypeConfiguration<JobCardPart>
{
    public void Configure(EntityTypeBuilder<JobCardPart> builder)
    {
        builder.HasKey(jp => jp.Id);

        builder.Property(jp => jp.UnitPriceAtTime).HasPrecision(18, 2);

        builder.HasOne(jp => jp.JobCard)
               .WithMany(j => j.JobCardParts)
               .HasForeignKey(jp => jp.JobCardId)
               .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(jp => jp.Part)
               .WithMany(p => p.JobCardParts)
               .HasForeignKey(jp => jp.PartId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
