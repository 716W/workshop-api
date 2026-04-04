using Workshop.Domain.Enums;

namespace Workshop.Application.DTOs;

public record UpdateServiceRequestStatusDto
{
    public ServiceRequestStatus NewStatus { get; init; }
    public string? Notes { get; init; }
}
