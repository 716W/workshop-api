using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Workshop.Domain.Entities;
using Workshop.Infrastructure.Identity;
using Workshop.Infrastructure.Persistence.Configurations;

namespace Workshop.Infrastructure.Persistence;

public class WorkshopDbContext : IdentityDbContext<ApplicationUser>
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
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<QuotationItem> QuotationItems => Set<QuotationItem>();
    public DbSet<PurchaseNeed> PurchaseNeeds => Set<PurchaseNeed>();
    public DbSet<ServiceRequestStatusHistory> ServiceRequestStatusHistories => Set<ServiceRequestStatusHistory>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<WorkerCommission> WorkerCommissions => Set<WorkerCommission>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically discovers and applies all IEntityTypeConfiguration<T>
        // implementations found in this assembly (Infrastructure layer).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkshopDbContext).Assembly);

        modelBuilder.SeedData();
    }
}
