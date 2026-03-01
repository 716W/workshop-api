namespace Workshop.Domain.Enums;

/// <summary>
/// Defines how a worker's commission is calculated on a service request.
/// </summary>
public enum CommissionType
{
    /// <summary>Worker receives a flat fixed monetary amount regardless of the request total.</summary>
    Fixed = 0,

    /// <summary>Worker receives a percentage of the request's total price.</summary>
    Percentage = 1
}
