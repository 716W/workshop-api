using Workshop.Domain.Enums;

namespace Workshop.Domain.Entities;

/// <summary>
/// A service request where the customer's vehicle is brought in for mechanical repair.
/// Requires a VehicleId because repair work is always tied to a specific vehicle.
/// </summary>
public sealed class RepairRequest : ServiceRequest
{
    public Guid VehicleId { get; private set; }
    public Vehicle Vehicle { get; private set; } = null!;

    /// <summary>Short description of the repair work to be performed.</summary>
    public string RepairDescription { get; private set; } = string.Empty;

    /// <summary>EF Core materialisation constructor.</summary>
    private RepairRequest() { }

    public RepairRequest(
        Guid customerId,
        Guid mechanicId,
        Guid vehicleId,
        string repairDescription,
        decimal price,
        CommissionType commissionType,
        decimal commissionValue)
        : base(customerId, mechanicId, price, commissionType, commissionValue, RequestType.Repair)
    {
        VehicleId = vehicleId;
        RepairDescription = repairDescription;
    }
}
