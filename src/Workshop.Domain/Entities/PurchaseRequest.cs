using Workshop.Domain.Enums;

namespace Workshop.Domain.Entities;

/// <summary>
/// A service request where the customer purchases parts only —
/// no vehicle or mechanical work is involved.
/// </summary>
public sealed class PurchaseRequest : ServiceRequest
{
    /// <summary>Free-text description of the parts being purchased.</summary>
    public string PurchaseDescription { get; private set; } = string.Empty;

    /// <summary>EF Core materialisation constructor.</summary>
    private PurchaseRequest() { }

    public PurchaseRequest(
        Guid customerId,
        Guid mechanicId,
        string purchaseDescription,
        decimal price,
        CommissionType commissionType,
        decimal commissionValue)
        : base(customerId, mechanicId, price, commissionType, commissionValue, RequestType.PurchaseOnly)
    {
        PurchaseDescription = purchaseDescription;
    }
}
