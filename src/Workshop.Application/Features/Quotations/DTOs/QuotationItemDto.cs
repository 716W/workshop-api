using Workshop.Domain.Enums;

namespace Workshop.Application.Features.Quotations.DTOs;

/// <summary>
/// Represents a single line item in a quotation request payload.
/// </summary>
public sealed record QuotationItemDto(
    /// <summary>Whether this item is a spare part or a labour charge.</summary>
    QuotationItemType Type,

    /// <summary>Human-readable description of the part or task.</summary>
    string Description,

    /// <summary>Number of units (e.g. pieces, hours). Must be >= 1.</summary>
    int Quantity,

    /// <summary>Price per unit. Must be >= 0.</summary>
    decimal UnitPrice
);
