using Microsoft.AspNetCore.Mvc;
using Workshop.API.Routes;
using Workshop.Application.Features.ServiceRequests.Commands;
using Workshop.Application.Features.ServiceRequests.DTOs;
using Workshop.Application.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Workshop.Domain.Common;

namespace Workshop.API.Controllers;

/// <summary>
/// Handles front-desk / reception operations: logging new customer service requests & releasing.
/// </summary>
[Authorize]
[Route(ApiRoutes.Reception.Base)]
public sealed class ReceptionController : BaseApiController
{
    private readonly ICommandHandler<CreateServiceRequestCommand, ServiceRequestCreatedResult> _handler;
    private readonly MediatR.IMediator _mediator;

    public ReceptionController(
        ICommandHandler<CreateServiceRequestCommand, ServiceRequestCreatedResult> handler,
        MediatR.IMediator mediator)
    {
        _handler = handler;
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new service request for a customer.
    /// The correct entity type (Repair, Purchase, Inspection) is selected automatically
    /// by the Factory based on the <c>requestType</c> field.
    /// </summary>
    /// <remarks>
    /// - **Repair**: VehicleId and Description are required.
    /// - **PurchaseOnly**: VehicleId is ignored; Description is required.
    /// - **InspectionOnly**: VehicleId and Description (inspection notes) are required.
    /// </remarks>
    /// <response code="201">Request created successfully. Returns the created resource summary.</response>
    /// <response code="400">Validation failed (missing required fields, invalid values, or business rule violation).</response>
    [HttpPost(ApiRoutes.Reception.Create)]
    [Authorize(Roles = "Receptionist,Manager")]
    [ProducesResponseType(typeof(Contracts.ApiResponse<ServiceRequestCreatedResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<ServiceRequestCreatedResult>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateServiceRequestDto dto,
        CancellationToken ct)
    {
        var command = new CreateServiceRequestCommand(dto);
        var result  = await _handler.HandleAsync(command, ct);

        // Build a Location URI pointing back to this action so the 201 header is meaningful.
        var location = Url.Action(nameof(Create)) ?? ApiRoutes.Reception.Base;
        return HandleCreated(result, location);
    }

    [HttpPost(ApiRoutes.Reception.Release)]
    [Authorize(Roles = "Receptionist,Manager")]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Release(Guid id)
    {
        var command = new CloseServiceRequestCommand(id);
        var result = await _mediator.Send(command);

        return HandleResult(result);
    }
}
