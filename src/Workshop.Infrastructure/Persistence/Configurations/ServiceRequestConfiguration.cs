using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Workshop.Domain.Entities;
using Workshop.Domain.Enums;

namespace Workshop.Infrastructure.Persistence.Configurations;

/// <summary>
/// EF Core Fluent API configuration for the ServiceRequest TPH hierarchy.
/// All subtypes (RepairRequest, PurchaseRequest, InspectionRequest) share the
/// "ServiceRequests" table and are distinguished by the "RequestType" discriminator column.
/// EF Core automatically adds nullable columns for subtype-specific properties.
/// </summary>
public sealed class ServiceRequestConfiguration : IEntityTypeConfiguration<ServiceRequest>
{
       public void Configure(EntityTypeBuilder<ServiceRequest> builder)
       {
              builder.ToTable("ServiceRequests");

              builder.HasKey(r => r.Id);

              // ── Discriminator (Table-Per-Hierarchy) ───────────────────────────
              builder.HasDiscriminator(r => r.RequestType)
                     .HasValue<RepairRequest>(RequestType.Repair)
                     .HasValue<PurchaseRequest>(RequestType.PurchaseOnly)
                     .HasValue<InspectionRequest>(RequestType.InspectionOnly);

              // ── Base columns ──────────────────────────────────────────────────
              builder.Property(r => r.Price).HasPrecision(18, 2).IsRequired();
              builder.Property(r => r.Date).IsRequired();
              builder.Property(r => r.CommissionType).HasConversion<string>().HasMaxLength(20);
              builder.Property(r => r.CommissionValue).HasPrecision(18, 4);
              builder.Property(r => r.RequestType).HasConversion<string>().HasMaxLength(30);

              // CommissionAmount is computed in memory — not mapped to a column.
              builder.Ignore(r => r.CommissionAmount);

              // ── Status ────────────────────────────────────────────────────────────
              builder.Property(r => r.Status)
                     .HasConversion<string>()
                     .HasMaxLength(40)
                     .HasDefaultValue(Workshop.Domain.Enums.ServiceRequestStatus.Open)
                     .IsRequired();

              // ── Quotation (1:0..1) ────────────────────────────────────────────────
              builder.HasOne(r => r.Quotation)
                     .WithOne(q => q.ServiceRequest)
                     .HasForeignKey<Workshop.Domain.Entities.Quotation>(q => q.ServiceRequestId)
                     .OnDelete(DeleteBehavior.Cascade);

              // ── Status Histories (1:N) ────────────────────────────────────────────
              builder.HasMany(r => r.StatusHistories)
                     .WithOne(h => h.ServiceRequest)
                     .HasForeignKey(h => h.ServiceRequestId)
                     .OnDelete(DeleteBehavior.Cascade);

              // ── Common relationships ──────────────────────────────────────────
              builder.HasOne(r => r.Customer)
                     .WithMany()
                     .HasForeignKey(r => r.CustomerId)
                     .OnDelete(DeleteBehavior.Restrict);

              builder.HasOne(r => r.Mechanic)
                     .WithMany()
                     .HasForeignKey(r => r.MechanicId)
                     .OnDelete(DeleteBehavior.Restrict);

              // ── Subtype string properties max-length constraints ──────────────
              // EF Core creates nullable columns automatically for each subtype property.
       }
}
