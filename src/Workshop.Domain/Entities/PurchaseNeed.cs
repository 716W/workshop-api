using Workshop.Domain.Common;

namespace Workshop.Domain.Entities;

/// <summary>
/// Records a purchasing requirement raised when a quoted part is out of stock
/// at the time of quotation approval. The purchasing department uses this
/// entity to restock inventory from an external supplier.
/// </summary>
public sealed class PurchaseNeed : BaseAuditableEntity
{
    // ── Part information ───────────────────────────────────────────────────────

    /// <summary>Name / description of the part that needs to be purchased.</summary>
    public string PartName { get; private set; } = string.Empty;

    /// <summary>How many units must be acquired to satisfy the job.</summary>
    public int Quantity { get; private set; }

    // ── Relationships ──────────────────────────────────────────────────────────

    /// <summary>The service request that triggered this purchase need.</summary>
    public Guid ServiceRequestId { get; private set; }

    /// <summary>
    /// Optional: the specific JobCard that will consume this part once it arrives.
    /// Null when the JobCard has not yet been created (e.g. at approval time).
    /// </summary>
    public Guid? JobCardId { get; private set; }

    // ── Audit ──────────────────────────────────────────────────────────────────

    /// <summary>UTC timestamp when the purchase need was registered.</summary>
    public DateTime DateRequested { get; private set; }

    // ── Constructors ───────────────────────────────────────────────────────────

    /// <summary>EF Core materialisation constructor.</summary>
    private PurchaseNeed() { }

    /// <summary>
    /// Creates a new purchase requirement for a part that is out of stock.
    /// </summary>
    /// <param name="partName">Human-readable part name.</param>
    /// <param name="quantity">Number of units required.</param>
    /// <param name="serviceRequestId">The triggering service request.</param>
    /// <param name="jobCardId">Optional associating job card.</param>
    public PurchaseNeed(string partName, int quantity, Guid serviceRequestId, Guid? jobCardId = null)
    {
        Id = Guid.NewGuid();
        PartName = partName;
        Quantity = quantity;
        ServiceRequestId = serviceRequestId;
        JobCardId = jobCardId;
        DateRequested = DateTime.UtcNow;
    }
}
