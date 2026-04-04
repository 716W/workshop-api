using Workshop.Domain.Common;

namespace Workshop.Application.Features.Quotations.Commands;

/// <summary>
/// CQRS Command: signals the intent to reject a quotation on behalf of the customer.
/// On success, the service request transitions to <c>Closed_Rejected</c>
/// and a pending inspection-fee invoice is automatically generated.
/// </summary>
/// <param name="QuotationId">The ID of the quotation to reject.</param>
public sealed record RejectQuotationCommand(Guid QuotationId);

/// <summary>The lightweight result returned to the caller on success.</summary>
public sealed record RejectQuotationResult(
    Guid QuotationId,
    Guid ServiceRequestId,
    Guid InvoiceId,
    decimal InspectionFee,
    string NewStatus);
