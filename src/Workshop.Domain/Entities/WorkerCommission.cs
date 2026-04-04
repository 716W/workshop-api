using Workshop.Domain.Common;

namespace Workshop.Domain.Entities;

/// <summary>
/// Represents a generic commission earned by a mechanic/worker for working on a service request.
/// </summary>
public class WorkerCommission : BaseAuditableEntity
{
    public Guid WorkerId { get; private set; }
    // Navigation property if needed, mapping to Mechanic or general Worker
    public Mechanic Worker { get; private set; } = null!;

    public Guid ServiceRequestId { get; private set; }
    public ServiceRequest ServiceRequest { get; private set; } = null!;

    /// <summary>Calculated monetary amount earned by the worker.</summary>
    public decimal Amount { get; private set; }

    private WorkerCommission() { } // EF Core

    public WorkerCommission(Guid workerId, Guid serviceRequestId, decimal amount)
    {
        Id = Guid.NewGuid();
        WorkerId = workerId;
        ServiceRequestId = serviceRequestId;
        Amount = amount;
        CreatedAt = DateTime.UtcNow;
    }
}
