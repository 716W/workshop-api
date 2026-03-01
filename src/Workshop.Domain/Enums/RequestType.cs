namespace Workshop.Domain.Enums;

/// <summary>
/// Discriminates the type of service a customer is requesting.
/// Drives the Factory Pattern to instantiate the correct ServiceRequest subtype.
/// </summary>
public enum RequestType
{
    /// <summary>Vehicle is brought in for mechanical repair work.</summary>
    Repair = 0,

    /// <summary>Customer is purchasing parts only — no vehicle service required.</summary>
    PurchaseOnly = 1,

    /// <summary>Vehicle is brought in for a diagnostic inspection only.</summary>
    InspectionOnly = 2
}
