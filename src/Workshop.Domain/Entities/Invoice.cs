using Workshop.Domain.Common;

namespace Workshop.Domain.Entities;

public class Invoice : BaseAuditableEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;

    public decimal PartsCost { get; set; }

    public decimal LaborCost { get; set; }

    public decimal TaxAmount { get; set; }

    public decimal TotalAmount { get; set; }

    public bool IsPaid { get; set; }

    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

    public DateTime? PaidAt { get; set; }

    // FK → JobCard (1:1)
    public Guid JobCardId { get; set; }
    public JobCard JobCard { get; set; } = null!;
}
