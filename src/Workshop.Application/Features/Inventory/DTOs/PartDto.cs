using System;

namespace Workshop.Application.Features.Inventory.DTOs;

public record PartDto(
    Guid Id,
    string Name,
    string PartNumber,
    string Description,
    decimal UnitPrice,
    int QuantityInStock,
    int ReorderLevel,
    DateTime CreatedAt
);
