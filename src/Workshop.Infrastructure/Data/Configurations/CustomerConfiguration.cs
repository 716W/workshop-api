using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Data.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.LastName).IsRequired().HasMaxLength(100);
        builder.Property(c => c.PhoneNumber).HasMaxLength(20);
        builder.Property(c => c.Email).HasMaxLength(200);

        builder.HasMany(c => c.Vehicles)
               .WithOne(v => v.Customer)
               .HasForeignKey(v => v.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
