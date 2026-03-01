namespace Workshop.Domain.Enums;

/// <summary>
/// Tracks the life-cycle state of a <see cref="Entities.ServiceRequest"/>.
/// </summary>
public enum ServiceRequestStatus
{
    /// <summary>Newly created; awaiting inspection / quotation.</summary>
    Open = 0,

    /// <summary>A quotation has been generated and is waiting for the customer's decision.</summary>
    PendingCustomerApproval = 1,

    /// <summary>The customer approved the quotation; work is in progress or completed.</summary>
    Approved = 2,

    /// <summary>Work is finished and the request has been fully closed.</summary>
    Closed = 3,

    /// <summary>The customer or workshop cancelled the request.</summary>
    Cancelled = 4
}
