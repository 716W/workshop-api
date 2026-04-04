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
    Cancelled = 4,

    /// <summary>The customer approved the quotation; active repair/service work is underway.</summary>
    In_Progress = 5,

    /// <summary>The customer rejected the quotation; an inspection fee invoice has been raised and the request is closed.</summary>
    Closed_Rejected = 6,

    /// <summary>Active mechanical or body repair work is currently being executed.</summary>
    Repairing = 7,

    /// <summary>The process is paused because parts ordered from an external supplier have not yet arrived.</summary>
    Waiting_For_Parts = 8,

    /// <summary>The vehicle has been sent to an external specialist (e.g., painting, specialized machine work).</summary>
    External_Work = 9,

    /// <summary>Repairs are completed and the vehicle is awaiting final Quality Control check.</summary>
    Ready_For_QC = 10
}
