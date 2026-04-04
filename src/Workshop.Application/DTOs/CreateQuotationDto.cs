namespace Workshop.Application.DTOs;

/// <summary>
/// Payload for the <c>POST /api/requests/{id}/quotations</c> endpoint.
/// </summary>
public sealed record CreateQuotationDto(
    /// <summary>
    /// One or more line items (parts and/or labour) that make up the quotation.
    /// The list must contain at least one item.
    /// </summary>
    IReadOnlyList<QuotationItemDto> Items,

    /// <summary>Optional free-text notes from the technician (max 2 000 characters).</summary>
    string? Notes
);
