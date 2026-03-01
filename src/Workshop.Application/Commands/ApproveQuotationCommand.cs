using Workshop.Domain.Common;

namespace Workshop.Application.Commands;

/// <summary>
/// CQRS Command: signals the intent to approve a specific quotation.
/// On success, the service request transitions to <c>In_Progress</c>
/// and a <c>QuotationApprovedEvent</c> is published.
/// </summary>
/// <param name="QuotationId">The ID of the quotation to approve.</param>
public sealed record ApproveQuotationCommand(Guid QuotationId);

/// <summary>The lightweight result returned to the caller on success.</summary>
public sealed record ApproveQuotationResult(
    Guid QuotationId,
    Guid ServiceRequestId,
    string NewStatus);
