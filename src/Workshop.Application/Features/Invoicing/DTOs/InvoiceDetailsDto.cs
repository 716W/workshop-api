using Workshop.Application.Features.ServiceRequests.DTOs;
using Workshop.Domain.Enums;

namespace Workshop.Application.Features.Invoicing.DTOs;

public record InvoiceDetailsDto(
    Guid Id,
    string InvoiceNumber,
    decimal SubTotal,
    decimal TaxAmount,
    decimal Discount,
    decimal TotalAmount,
    InvoiceStatus Status,
    DateTime IssuedAt,
    DateTime? PaidAt,
    List<PaymentDto> Payments,
    ServiceRequestLinkedDto? ServiceRequest
);

public record PaymentDto(
    Guid Id,
    decimal Amount,
    PaymentMethod PaymentMethod,
    DateTime PaymentDate,
    string TransactionReference
);

public record ServiceRequestLinkedDto(
    Guid Id,
    RequestType RequestType,
    ServiceRequestStatus Status,
    CustomerDetailsDto Customer,
    VehicleDetailsDto? Vehicle,
    QuotationDetailsDto? Quotation
);
