using Workshop.Domain.Common;
using Workshop.Domain.Enums;

namespace Workshop.Domain.Entities;

/// <summary>
/// Represents a single line item inside a <see cref="Quotation"/>.
/// Each item is either a physical part or a labour charge.
/// </summary>
public sealed class QuotationItem : BaseAuditableEntity
{
    /// <summary>Foreign key to the parent <see cref="Entities.Quotation"/>.</summary>
    public Guid QuotationId { get; private set; }

    /// <summary>Navigation property to the parent quotation.</summary>
    public Quotation Quotation { get; private set; } = null!;

    /// <summary>Whether this line item represents a spare part or labour time.</summary>
    public QuotationItemType Type { get; private set; }

    /// <summary>Human-readable description of the part or labour task.</summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>Number of units (pieces, hours, etc.).</summary>
    public int Quantity { get; private set; }

    /// <summary>Price per single unit, excluding any mark-up or discount.</summary>
    public decimal UnitPrice { get; private set; }

    /// <summary>
    /// Pre-computed total: <see cref="Quantity"/> × <see cref="UnitPrice"/>.
    /// Stored in the database for reporting convenience.
    /// </summary>
    public decimal TotalPrice { get; private set; }

    /// <summary>EF Core materialisation constructor.</summary>
    private QuotationItem() { }

    public QuotationItem(
        QuotationItemType type,
        string description,
        int quantity,
        decimal unitPrice)
    {
        Id = Guid.NewGuid();
        Type = type;
        Description = description;
        Quantity = quantity;
        UnitPrice = unitPrice;
        TotalPrice = quantity * unitPrice;
    }
}
