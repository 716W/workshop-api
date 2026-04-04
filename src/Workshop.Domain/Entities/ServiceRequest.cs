using Workshop.Domain.Common;
using Workshop.Domain.Enums;
using Workshop.Domain.Exceptions;

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

    // ── Status ────────────────────────────────────────────────────────────────

    /// <summary>Current life-cycle state of this service request.</summary>
    public ServiceRequestStatus Status { get; protected set; } = ServiceRequestStatus.Open;

    // ── Status History (1:N) ──────────────────────────────────────────────────

    private readonly List<ServiceRequestStatusHistory> _statusHistories = new();
    public IReadOnlyCollection<ServiceRequestStatusHistory> StatusHistories => _statusHistories.AsReadOnly();

    // ── Quotation (1:0..1) ────────────────────────────────────────────────────

    /// <summary>
    /// The quotation attached to this request, if one has been generated.
    /// Null when the request is still in the <see cref="ServiceRequestStatus.Open"/> state.
    /// </summary>
    public Quotation? Quotation { get; protected set; }

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

    // ── Domain Behaviours ─────────────────────────────────────────────────────

    /// <summary>
    /// Attaches a <see cref="Entities.Quotation"/> to this request and transitions the
    /// status to <see cref="ServiceRequestStatus.PendingCustomerApproval"/>.
    /// </summary>
    /// <exception cref="BusinessRuleException">
    /// Thrown when the request is already <see cref="ServiceRequestStatus.Closed"/>
    /// or <see cref="ServiceRequestStatus.Cancelled"/> — terminal states that forbid
    /// any further modifications.
    /// </exception>
    public void AttachQuotation(Quotation quotation)
    {
        if (Status == ServiceRequestStatus.Closed)
            throw new BusinessRuleException(
                $"Cannot attach a quotation to a closed service request (Id: {Id}).");

        if (Status == ServiceRequestStatus.Cancelled)
            throw new BusinessRuleException(
                $"Cannot attach a quotation to a cancelled service request (Id: {Id}).");

        Quotation = quotation;
        Status = ServiceRequestStatus.PendingCustomerApproval;
    }

    /// <summary>
    /// Records the customer's approval of the attached quotation and starts work.
    /// Transitions the status to <see cref="ServiceRequestStatus.In_Progress"/>.
    /// </summary>
    /// <exception cref="BusinessRuleException">
    /// Thrown when the request is not in <see cref="ServiceRequestStatus.PendingCustomerApproval"/>.
    /// </exception>
    public void ApproveQuotation()
    {
        if (Status != ServiceRequestStatus.PendingCustomerApproval)
            throw new BusinessRuleException(
                $"Quotation can only be approved when the request is in '{ServiceRequestStatus.PendingCustomerApproval}' status. " +
                $"Current status: '{Status}' (Id: {Id}).");

        if (Quotation is null)
            throw new BusinessRuleException(
                $"No quotation is attached to service request (Id: {Id}). Cannot approve.");

        Quotation.Approve();
        Status = ServiceRequestStatus.In_Progress;
    }

    /// <summary>
    /// Records the customer's rejection of the attached quotation and closes the request.
    /// Transitions the status to <see cref="ServiceRequestStatus.Closed_Rejected"/>.
    /// An inspection-fee invoice must be created by the caller.
    /// </summary>
    /// <exception cref="BusinessRuleException">
    /// Thrown when the request is not in <see cref="ServiceRequestStatus.PendingCustomerApproval"/>.
    /// </exception>
    public void RejectQuotation()
    {
        if (Status != ServiceRequestStatus.PendingCustomerApproval)
            throw new BusinessRuleException(
                $"Quotation can only be rejected when the request is in '{ServiceRequestStatus.PendingCustomerApproval}' status. " +
                $"Current status: '{Status}' (Id: {Id}).");

        if (Quotation is null)
            throw new BusinessRuleException(
                $"No quotation is attached to service request (Id: {Id}). Cannot reject.");

        Quotation.Reject();
        Status = ServiceRequestStatus.Closed_Rejected;
    }

    /// <summary>
    /// Changes the status of this request and records a history entry.
    /// </summary>
    public void ChangeStatus(ServiceRequestStatus newStatus, string? notes)
    {
        if (Status == newStatus)
            return;

        var history = new ServiceRequestStatusHistory(
            Id,
            Status,
            newStatus,
            notes);

        _statusHistories.Add(history);
        Status = newStatus;
    }
}
