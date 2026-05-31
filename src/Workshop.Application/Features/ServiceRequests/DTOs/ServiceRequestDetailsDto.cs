using System;
using System.Collections.Generic;
using Workshop.Domain.Enums;

namespace Workshop.Application.Features.ServiceRequests.DTOs;

public record ServiceRequestDetailsDto(
    Guid Id,
    DateTime Date,
    RequestType RequestType,
    ServiceRequestStatus Status,
    decimal Price,
    DateTime? ClosedAt,
    string? Notes,
    CustomerDetailsDto Customer,
    VehicleDetailsDto? Vehicle,
    QuotationDetailsDto? Quotation,
    InvoiceDetailsDto? Invoice,
    List<StatusHistoryDto> StatusHistory
);

public record CustomerDetailsDto(
    Guid Id,
    string Name,
    string Email,
    string PhoneNumber
);

public record VehicleDetailsDto(
    Guid Id,
    string Make,
    string Model,
    int Year,
    string PlateNumber
);

public record QuotationDetailsDto(
    Guid Id,
    decimal GrandTotal,
    string? Notes,
    QuotationStatus Status,
    List<QuotationItemDetailsDto> Items
);

public record QuotationItemDetailsDto(
    Guid Id,
    QuotationItemType Type,
    string Description,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice
);

public record InvoiceDetailsDto(
    Guid Id,
    string InvoiceNumber,
    decimal SubTotal,
    decimal TaxAmount,
    decimal Discount,
    decimal TotalAmount,
    InvoiceStatus Status,
    DateTime IssuedAt,
    DateTime? PaidAt
);

public record StatusHistoryDto(
    Guid Id,
    ServiceRequestStatus OldStatus,
    ServiceRequestStatus NewStatus,
    string? Notes,
    DateTime CreatedAt
);
