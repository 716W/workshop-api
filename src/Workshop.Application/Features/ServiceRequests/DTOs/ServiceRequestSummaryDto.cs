using System;
using Workshop.Domain.Enums;

namespace Workshop.Application.Features.ServiceRequests.DTOs;

public record ServiceRequestSummaryDto(
    Guid Id,
    DateTime Date,
    string CustomerName,
    string VehiclePlate,
    RequestType RequestType,
    ServiceRequestStatus Status,
    decimal TotalAmount
);
