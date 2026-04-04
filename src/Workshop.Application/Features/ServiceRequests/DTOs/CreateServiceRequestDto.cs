using Workshop.Domain.Enums;

namespace Workshop.Application.Features.ServiceRequests.DTOs;

/// <summary>
/// Payload sent by the client to create a new ServiceRequest.
/// Validation rules are type-dependent (see CreateServiceRequestValidator).
/// </summary>
public sealed class CreateServiceRequestDto
{
    /// <summary>Discriminates which kind of service request to create.</summary>
    public RequestType RequestType { get; init; }

    public Guid CustomerId { get; init; }
    public Guid MechanicId { get; init; }

    /// <summary>
    /// Required for Repair and InspectionOnly request types.
    /// Optional / ignored for PurchaseOnly.
    /// </summary>
    public Guid? VehicleId { get; init; }

    /// <summary>Total price of the service/purchase.</summary>
    public decimal Price { get; init; }

    /// <summary>Defines how the worker's commission is calculated.</summary>
    public CommissionType CommissionType { get; init; }

    /// <summary>
    /// The commission amount or percentage value.
    /// For Fixed: absolute monetary amount. For Percentage: value between 0 and 100.
    /// </summary>
    public decimal CommissionValue { get; init; }

    /// <summary>
    /// Description of the repair work — required for Repair requests.
    /// Used as RepairDescription on RepairRequest.
    /// </summary>
    public string? Description { get; init; }
}
