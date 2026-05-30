namespace Workshop.Application.Features.Workers.DTOs;

public record WorkerCommissionDto(
    Guid Id,
    decimal Amount,
    DateTime CreatedAt,
    Guid ServiceRequestId
);
