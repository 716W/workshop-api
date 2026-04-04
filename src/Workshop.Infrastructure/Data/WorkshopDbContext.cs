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
    public DbSet<ServiceRequest> ServiceRequests => Set<ServiceRequest>();
    public DbSet<Quotation> Quotations => Set<Quotation>();
    public DbSet<QuotationItem> QuotationItems => Set<QuotationItem>();
    public DbSet<PurchaseNeed> PurchaseNeeds => Set<PurchaseNeed>();
    public DbSet<ServiceRequestStatusHistory> ServiceRequestStatusHistories => Set<ServiceRequestStatusHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Automatically discovers and applies all IEntityTypeConfiguration<T>
        // implementations found in this assembly (Infrastructure layer).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WorkshopDbContext).Assembly);
    }
}
