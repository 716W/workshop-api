using Workshop.Application.DTOs;
using Workshop.Domain.Common;

namespace Workshop.Application.Commands;

/// <summary>
/// CQRS Command: signals the intent to create a new ServiceRequest.
/// Wraps the validated DTO so the handler only depends on well-defined input.
/// </summary>
/// <param name="Dto">The validated creation payload.</param>
public sealed record CreateServiceRequestCommand(CreateServiceRequestDto Dto);

/// <summary>Expected result returned by the handler on success.</summary>
public sealed record ServiceRequestCreatedResult(
    Guid Id,
    string RequestType,
    decimal Price,
    decimal CommissionAmount);
