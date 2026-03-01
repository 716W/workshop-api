namespace Workshop.Domain.Enums;

/// <summary>
/// Classifies a line item inside a quotation.
/// </summary>
public enum QuotationItemType
{
    /// <summary>A physical part / spare part used in the repair or service.</summary>
    Part = 0,

    /// <summary>Labour / workshop time charged to the customer.</summary>
    Labor = 1
}
