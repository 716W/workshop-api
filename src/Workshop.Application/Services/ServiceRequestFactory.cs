using Workshop.Application.DTOs;
using Workshop.Application.Interfaces;
using Workshop.Domain.Entities;
using Workshop.Domain.Enums;
using Workshop.Domain.Exceptions;

namespace Workshop.Application.Services;

/// <summary>
/// Concrete implementation of <see cref="IServiceRequestFactory"/>.
///
/// The Factory Method Pattern encapsulates the construction logic so that:
/// 1. Callers never need to know which concrete <see cref="ServiceRequest"/> subtype to instantiate.
/// 2. Adding a new <see cref="RequestType"/> only requires adding a new case here and a new entity
///    — all existing consumers remain untouched (Open-Closed Principle).
/// </summary>
public sealed class ServiceRequestFactory : IServiceRequestFactory
{
    /// <inheritdoc />
    public ServiceRequest Create(CreateServiceRequestDto dto)
    {
        return dto.RequestType switch
        {
            RequestType.Repair => CreateRepairRequest(dto),
            RequestType.PurchaseOnly => CreatePurchaseRequest(dto),
            RequestType.InspectionOnly => CreateInspectionRequest(dto),
            _ => throw new BusinessRuleException(
                     $"RequestType '{dto.RequestType}' is not supported by the factory.")
        };
    }

    // ── Private factory methods ───────────────────────────────────────────────

    private static RepairRequest CreateRepairRequest(CreateServiceRequestDto dto)
    {
        if (dto.VehicleId is null || dto.VehicleId == Guid.Empty)
            throw new BusinessRuleException("VehicleId is required for a Repair request.");

        return new RepairRequest(
            customerId: dto.CustomerId,
            mechanicId: dto.MechanicId,
            vehicleId: dto.VehicleId.Value,
            repairDescription: dto.Description ?? string.Empty,
            price: dto.Price,
            commissionType: dto.CommissionType,
            commissionValue: dto.CommissionValue);
    }

    private static PurchaseRequest CreatePurchaseRequest(CreateServiceRequestDto dto)
    {
        return new PurchaseRequest(
            customerId: dto.CustomerId,
            mechanicId: dto.MechanicId,
            purchaseDescription: dto.Description ?? string.Empty,
            price: dto.Price,
            commissionType: dto.CommissionType,
            commissionValue: dto.CommissionValue);
    }

    private static InspectionRequest CreateInspectionRequest(CreateServiceRequestDto dto)
    {
        if (dto.VehicleId is null || dto.VehicleId == Guid.Empty)
            throw new BusinessRuleException("VehicleId is required for an InspectionOnly request.");

        return new InspectionRequest(
            customerId: dto.CustomerId,
            mechanicId: dto.MechanicId,
            vehicleId: dto.VehicleId.Value,
            inspectionNotes: dto.Description ?? string.Empty,
            price: dto.Price,
            commissionType: dto.CommissionType,
            commissionValue: dto.CommissionValue);
    }
}
