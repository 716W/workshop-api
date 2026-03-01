using Workshop.Domain.Common;

namespace Workshop.Domain.Entities;

/// <summary>
/// A cost estimate presented to the customer before work begins.
/// A <see cref="Quotation"/> belongs to exactly one <see cref="ServiceRequest"/>
/// and contains one or more <see cref="QuotationItem"/> entries.
/// </summary>
public sealed class Quotation : BaseAuditableEntity
{
    // ── Relationships ─────────────────────────────────────────────────────────

    /// <summary>Foreign key to the owning service request.</summary>
    public Guid ServiceRequestId { get; private set; }

    /// <summary>Navigation property to the owning service request.</summary>
    public ServiceRequest ServiceRequest { get; private set; } = null!;

    // ── Money ─────────────────────────────────────────────────────────────────

    /// <summary>Sum of all <see cref="QuotationItem.TotalPrice"/> values.</summary>
    public decimal GrandTotal { get; private set; }

    // ── Items ─────────────────────────────────────────────────────────────────

    private readonly List<QuotationItem> _items = [];

    /// <summary>Read-only view of the line items on this quotation.</summary>
    public IReadOnlyCollection<QuotationItem> Items => _items.AsReadOnly();

    // ── Notes ─────────────────────────────────────────────────────────────────

    /// <summary>Optional technician remarks accompanying the quotation.</summary>
    public string? Notes { get; private set; }

    // ── Constructors ──────────────────────────────────────────────────────────

    /// <summary>EF Core materialisation constructor.</summary>
    private Quotation() { }

    /// <summary>
    /// Creates a new Quotation for the specified service request with a pre-validated list of items.
    /// EF Core's change tracker will automatically populate <see cref="QuotationItem.QuotationId"/>
    /// when the entity graph is saved.
    /// </summary>
    public Quotation(Guid serviceRequestId, IEnumerable<QuotationItem> items, string? notes = null)
    {
        Id = Guid.NewGuid();
        ServiceRequestId = serviceRequestId;
        Notes = notes;
        _items.AddRange(items);
        GrandTotal = _items.Sum(i => i.TotalPrice);
    }
}
