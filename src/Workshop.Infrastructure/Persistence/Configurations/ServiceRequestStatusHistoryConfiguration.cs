using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Persistence.Configurations;

public sealed class ServiceRequestStatusHistoryConfiguration : IEntityTypeConfiguration<ServiceRequestStatusHistory>
{
    public void Configure(EntityTypeBuilder<ServiceRequestStatusHistory> builder)
    {
        builder.ToTable("ServiceRequestStatusHistories");
        builder.HasKey(h => h.Id);

        builder.Property(h => h.OldStatus)
               .HasConversion<string>()
               .HasMaxLength(40)
               .IsRequired();

        builder.Property(h => h.NewStatus)
               .HasConversion<string>()
               .HasMaxLength(40)
               .IsRequired();

        builder.Property(h => h.Notes)
               .HasMaxLength(1000);
    }
}
