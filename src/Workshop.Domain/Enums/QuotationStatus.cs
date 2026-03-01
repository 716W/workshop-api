namespace Workshop.Domain.Enums;

/// <summary>
/// Tracks the customer's decision on a <see cref="Entities.Quotation"/>.
/// </summary>
public enum QuotationStatus
{
    /// <summary>Generated and sent to the customer; awaiting their decision.</summary>
    Pending = 0,

    /// <summary>The customer accepted the quotation; work can begin.</summary>
    Approved = 1,

    /// <summary>The customer declined the quotation; an inspection fee is applied.</summary>
    Rejected = 2
}
