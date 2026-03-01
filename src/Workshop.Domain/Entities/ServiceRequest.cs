using Workshop.Domain.Common;
using Workshop.Domain.Enums;

namespace Workshop.Domain.Entities;

/// <summary>
/// Abstract base for every type of customer service request.
/// EF Core uses Table-Per-Hierarchy (TPH) — a single "ServiceRequests" table
/// with a "RequestType" discriminator column for all subtypes.
/// </summary>
public abstract class ServiceRequest : BaseAuditableEntity
{
    /// <summary>Total price charged to the customer for this request.</summary>
    public decimal Price { get; protected set; }

    /// <summary>Date/time the request was registered.</summary>
    public DateTime Date { get; protected set; }

    // ── Relationships ────────────────────────────────────────────────────────

    public Guid CustomerId { get; protected set; }
    public Customer Customer { get; protected set; } = null!;

    /// <summary>The mechanic/worker responsible for handling this request.</summary>
    public Guid MechanicId { get; protected set; }
    public Mechanic Mechanic { get; protected set; } = null!;

    // ── Commission ───────────────────────────────────────────────────────────

    /// <summary>How the commission is calculated (Fixed amount or Percentage of price).</summary>
    public CommissionType CommissionType { get; protected set; }

    /// <summary>
    /// The commission value. Interpreted as a flat amount when <see cref="CommissionType"/> is Fixed,
    /// or as a percentage (0–100) when it is Percentage.
    /// </summary>
    public decimal CommissionValue { get; protected set; }

    /// <summary>
    /// Computed: returns the actual monetary commission earned by the worker.
    /// Not persisted to the database (computed in memory).
    /// </summary>
    public decimal CommissionAmount => CommissionType == Enums.CommissionType.Fixed
        ? CommissionValue
        : Price * CommissionValue / 100m;

    // ── Discriminator (set by derived constructors) ───────────────────────────
    public RequestType RequestType { get; protected set; }

    /// <summary>EF Core requires a parameterless constructor for materialisation.</summary>
    protected ServiceRequest() { }

    protected ServiceRequest(
        Guid customerId,
        Guid mechanicId,
        decimal price,
        CommissionType commissionType,
        decimal commissionValue,
        RequestType requestType)
    {
        Id = Guid.NewGuid();
        CustomerId = customerId;
        MechanicId = mechanicId;
        Price = price;
        CommissionType = commissionType;
        CommissionValue = commissionValue;
        RequestType = requestType;
        Date = DateTime.UtcNow;
    }
}
