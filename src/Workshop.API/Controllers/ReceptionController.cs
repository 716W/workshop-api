using Microsoft.AspNetCore.Mvc;
using Workshop.Application.Commands;
using Workshop.Application.DTOs;
using Workshop.Application.Interfaces;

namespace Workshop.API.Controllers;

/// <summary>
/// Handles front-desk / reception operations: logging new customer service requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public sealed class ReceptionController : ControllerBase
{
    private readonly ICommandHandler<CreateServiceRequestCommand, ServiceRequestCreatedResult> _handler;

    public ReceptionController(
        ICommandHandler<CreateServiceRequestCommand, ServiceRequestCreatedResult> handler)
    {
        _handler = handler;
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
    /// <response code="400">Validation failed (missing required fields or invalid values).</response>
    /// <response code="422">Business rule violation (e.g. vehicle not found, invalid state).</response>
    [HttpPost("create")]
    [ProducesResponseType(typeof(ServiceRequestCreatedResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateServiceRequestDto dto,
        CancellationToken ct)
    {
        // FluentValidation auto-validation runs before this action body executes.
        // If ModelState is invalid, ASP.NET Core returns 400 automatically.

        var command = new CreateServiceRequestCommand(dto);
        var result = await _handler.HandleAsync(command, ct);

        if (!result.IsSuccess)
        {
            // Handler captured an unexpected failure — surface it as a problem detail.
            return Problem(
                detail: result.Error,
                statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return CreatedAtAction(
            actionName: nameof(Create),
            value: result.Value);
    }
}
