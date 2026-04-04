using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Data.Configurations;

public sealed class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(v => v.Id);

        builder.Property(v => v.Make).IsRequired().HasMaxLength(50);
        builder.Property(v => v.Model).IsRequired().HasMaxLength(50);
        builder.Property(v => v.PlateNumber).IsRequired().HasMaxLength(20);
        builder.Property(v => v.VinNumber).HasMaxLength(50);

        builder.HasIndex(v => v.PlateNumber).IsUnique();

        builder.HasMany(v => v.JobCards)
               .WithOne(j => j.Vehicle)
               .HasForeignKey(j => j.VehicleId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
