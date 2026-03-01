using MediatR;

namespace Workshop.Domain.Events;

/// <summary>
/// Domain event published when a customer approves a quotation.
/// Handlers (e.g. <c>AllocatePartsEventHandler</c>) react to this event
/// to perform inventory checks and create purchase needs for out-of-stock parts.
/// </summary>
/// <param name="QuotationId">The ID of the approved quotation.</param>
/// <param name="ServiceRequestId">The ID of the owning service request.</param>
public sealed record QuotationApprovedEvent(
    Guid QuotationId,
    Guid ServiceRequestId) : INotification;
