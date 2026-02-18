using Microsoft.EntityFrameworkCore;
using Workshop.Domain.Entities;

namespace Workshop.Infrastructure.Data;

public class WorkshopDbContext : DbContext
{
    public WorkshopDbContext(DbContextOptions<WorkshopDbContext> options)
        : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<Mechanic> Mechanics => Set<Mechanic>();
    public DbSet<Part> Parts => Set<Part>();
    public DbSet<JobCard> JobCards => Set<JobCard>();
    public DbSet<JobCardPart> JobCardParts => Set<JobCardPart>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ── Customer ──────────────────────────────────────────────
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.LastName).IsRequired().HasMaxLength(100);
            entity.Property(c => c.PhoneNumber).HasMaxLength(20);
            entity.Property(c => c.Email).HasMaxLength(200);

            entity.HasMany(c => c.Vehicles)
                  .WithOne(v => v.Customer)
                  .HasForeignKey(v => v.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Vehicle ───────────────────────────────────────────────
        modelBuilder.Entity<Vehicle>(entity =>
        {
            entity.HasKey(v => v.Id);
            entity.Property(v => v.Make).IsRequired().HasMaxLength(50);
            entity.Property(v => v.Model).IsRequired().HasMaxLength(50);
            entity.Property(v => v.PlateNumber).IsRequired().HasMaxLength(20);
            entity.Property(v => v.VinNumber).HasMaxLength(50);

            entity.HasIndex(v => v.PlateNumber).IsUnique();

            entity.HasMany(v => v.JobCards)
                  .WithOne(j => j.Vehicle)
                  .HasForeignKey(j => j.VehicleId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Mechanic ──────────────────────────────────────────────
        modelBuilder.Entity<Mechanic>(entity =>
        {
            entity.HasKey(m => m.Id);
            entity.Property(m => m.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(m => m.LastName).IsRequired().HasMaxLength(100);
            entity.Property(m => m.Specialization).HasMaxLength(100);
            entity.Property(m => m.PhoneNumber).HasMaxLength(20);
        });

        // ── Part ──────────────────────────────────────────────────
        modelBuilder.Entity<Part>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(150);
            entity.Property(p => p.PartNumber).IsRequired().HasMaxLength(50);
            entity.Property(p => p.UnitPrice).HasPrecision(18, 2);

            entity.HasIndex(p => p.PartNumber).IsUnique();
        });

        // ── JobCard ───────────────────────────────────────────────
        modelBuilder.Entity<JobCard>(entity =>
        {
            entity.HasKey(j => j.Id);
            entity.Property(j => j.JobNumber).IsRequired().HasMaxLength(30);
            entity.Property(j => j.Description).HasMaxLength(500);
            entity.Property(j => j.InspectionNotes).HasMaxLength(2000);
            entity.Property(j => j.EstimatedCost).HasPrecision(18, 2);
            entity.Property(j => j.FinalCost).HasPrecision(18, 2);
            entity.Property(j => j.Status).HasConversion<string>().HasMaxLength(30);

            entity.HasIndex(j => j.JobNumber).IsUnique();

            entity.HasOne(j => j.Mechanic)
                  .WithMany(m => m.JobCards)
                  .HasForeignKey(j => j.MechanicId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(j => j.Invoice)
                  .WithOne(i => i.JobCard)
                  .HasForeignKey<Invoice>(i => i.JobCardId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // ── JobCardPart (Join Table) ──────────────────────────────
        modelBuilder.Entity<JobCardPart>(entity =>
        {
            entity.HasKey(jp => jp.Id);
            entity.Property(jp => jp.UnitPriceAtTime).HasPrecision(18, 2);

            entity.HasOne(jp => jp.JobCard)
                  .WithMany(j => j.JobCardParts)
                  .HasForeignKey(jp => jp.JobCardId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(jp => jp.Part)
                  .WithMany(p => p.JobCardParts)
                  .HasForeignKey(jp => jp.PartId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ── Invoice ───────────────────────────────────────────────
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.InvoiceNumber).IsRequired().HasMaxLength(30);
            entity.Property(i => i.PartsCost).HasPrecision(18, 2);
            entity.Property(i => i.LaborCost).HasPrecision(18, 2);
            entity.Property(i => i.TaxAmount).HasPrecision(18, 2);
            entity.Property(i => i.TotalAmount).HasPrecision(18, 2);

            entity.HasIndex(i => i.InvoiceNumber).IsUnique();
        });
    }
}
