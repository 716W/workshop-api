using Workshop.Application.DTOs;
using Workshop.Domain.Entities;

namespace Workshop.Application.Interfaces;

/// <summary>
/// Factory contract for creating the correct <see cref="ServiceRequest"/> subtype
/// based on the <see cref="CreateServiceRequestDto.RequestType"/>.
///
/// Applying the Open-Closed Principle: adding a new request type only requires
/// adding a new subtype and registering it in the factory — no existing code changes.
/// </summary>
public interface IServiceRequestFactory
{
    /// <summary>
    /// Instantiates the appropriate <see cref="ServiceRequest"/> subclass
    /// (e.g. <see cref="RepairRequest"/>, <see cref="PurchaseRequest"/>, <see cref="InspectionRequest"/>)
    /// from the incoming DTO.
    /// </summary>
    /// <param name="dto">Validated creation payload.</param>
    /// <returns>A fully initialised but not-yet-persisted domain entity.</returns>
    ServiceRequest Create(CreateServiceRequestDto dto);
}
