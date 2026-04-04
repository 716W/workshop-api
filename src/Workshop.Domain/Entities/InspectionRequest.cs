using Workshop.Domain.Enums;

namespace Workshop.Domain.Entities;

/// <summary>
/// A service request where the customer's vehicle is brought in for a
/// diagnostic inspection only — no repair work is performed.
/// </summary>
public sealed class InspectionRequest : ServiceRequest
{
    public Guid VehicleId { get; private set; }
    public Vehicle Vehicle { get; private set; } = null!;

    /// <summary>Notes recorded by the mechanic during the inspection.</summary>
    public string InspectionNotes { get; private set; } = string.Empty;

    /// <summary>EF Core materialisation constructor.</summary>
    private InspectionRequest() { }

    public InspectionRequest(
        Guid customerId,
        Guid mechanicId,
        Guid vehicleId,
        string inspectionNotes,
        decimal price,
        CommissionType commissionType,
        decimal commissionValue)
        : base(customerId, mechanicId, price, commissionType, commissionValue, RequestType.InspectionOnly)
    {
        VehicleId = vehicleId;
        InspectionNotes = inspectionNotes;
    }
}
