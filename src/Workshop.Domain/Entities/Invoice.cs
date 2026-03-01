using Workshop.Domain.Common;

namespace Workshop.Domain.Entities;

/// <summary>
/// A financial document issued to the customer.
/// An invoice can originate from a completed job (<see cref="JobCard"/>) or
/// from a rejected quotation (inspection fee only), so both FKs are optional.
/// </summary>
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

    // ── Relationships ──────────────────────────────────────────────────────────

    /// <summary>
    /// FK → JobCard (nullable). Populated for job-completion invoices.
    /// Null for inspection-fee invoices raised on quotation rejection.
    /// </summary>
    public Guid? JobCardId { get; set; }
    public JobCard? JobCard { get; set; }

    /// <summary>
    /// FK → ServiceRequest (nullable). Populated for inspection-fee invoices
    /// raised when a customer rejects a quotation. Null for job-completion invoices.
    /// </summary>
    public Guid? ServiceRequestId { get; set; }
    public ServiceRequest? ServiceRequest { get; set; }

    // ── Factory method ─────────────────────────────────────────────────────────

    /// <summary>
    /// Creates a pending inspection-fee invoice linked directly to a service request.
    /// Used when the customer rejects the quotation.
    /// </summary>
    /// <param name="serviceRequestId">The ID of the rejected service request.</param>
    /// <param name="inspectionFeeAmount">The fixed inspection fee to charge.</param>
    public static Invoice CreateInspectionFeeInvoice(Guid serviceRequestId, decimal inspectionFeeAmount)
    {
        return new Invoice
        {
            Id = Guid.NewGuid(),
            InvoiceNumber = $"INS-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}",
            PartsCost = 0m,
            LaborCost = inspectionFeeAmount,
            TaxAmount = 0m,
            TotalAmount = inspectionFeeAmount,
            IsPaid = false,
            IssuedAt = DateTime.UtcNow,
            ServiceRequestId = serviceRequestId,
            JobCardId = null
        };
    }
}
