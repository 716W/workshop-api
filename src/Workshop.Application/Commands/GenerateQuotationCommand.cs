using Workshop.Application.DTOs;
using Workshop.Domain.Common;

namespace Workshop.Application.Commands;

/// <summary>
/// CQRS Command: signals the intent to generate a quotation for an existing service request.
/// </summary>
/// <param name="ServiceRequestId">The ID of the service request to attach the quotation to.</param>
/// <param name="Dto">The validated quotation payload.</param>
public sealed record GenerateQuotationCommand(
    Guid ServiceRequestId,
    CreateQuotationDto Dto);

/// <summary>The lightweight result returned to the caller on success.</summary>
public sealed record QuotationGeneratedResult(
    Guid QuotationId,
    Guid ServiceRequestId,
    decimal GrandTotal,
    string NewStatus);
