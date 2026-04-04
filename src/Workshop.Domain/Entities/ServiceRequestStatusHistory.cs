using Workshop.Domain.Common;
using Workshop.Domain.Enums;

namespace Workshop.Domain.Entities;

/// <summary>
/// Tracks every status change of a <see cref="ServiceRequest"/> providing an
/// audit trail of how and when the repair progressed.
/// </summary>
public class ServiceRequestStatusHistory : BaseAuditableEntity
{
    public Guid ServiceRequestId { get; private set; }
    public ServiceRequest ServiceRequest { get; private set; } = null!;

    public ServiceRequestStatus OldStatus { get; private set; }
    public ServiceRequestStatus NewStatus { get; private set; }
    public string? Notes { get; private set; }

    /// <summary>EF Core parameterless constructor</summary>
    private ServiceRequestStatusHistory() { }

    public ServiceRequestStatusHistory(
        Guid serviceRequestId,
        ServiceRequestStatus oldStatus,
        ServiceRequestStatus newStatus,
        string? notes)
    {
        Id = Guid.NewGuid();
        ServiceRequestId = serviceRequestId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        Notes = notes;
    }
}
