using Microsoft.AspNetCore.Mvc;
using Workshop.Application.Commands;
using Workshop.Application.DTOs;
using Workshop.Application.Interfaces;

namespace Workshop.API.Controllers;

/// <summary>
/// Handles workshop-floor operations such as generating quotations for service requests.
/// </summary>
[ApiController]
[Route("api/requests")]
public sealed class OperationsController : ControllerBase
{
    private readonly ICommandHandler<GenerateQuotationCommand, QuotationGeneratedResult> _generateQuotationHandler;
    private readonly ICommandHandler<UpdateServiceRequestStatusCommand, Guid> _updateStatusHandler;

    public OperationsController(
        ICommandHandler<GenerateQuotationCommand, QuotationGeneratedResult> generateQuotationHandler,
        ICommandHandler<UpdateServiceRequestStatusCommand, Guid> updateStatusHandler)
    {
        _generateQuotationHandler = generateQuotationHandler;
        _updateStatusHandler = updateStatusHandler;
    }

    /// <summary>
    /// Generates a quotation for an existing service request and transitions its
    /// status to <c>PendingCustomerApproval</c>.
    /// </summary>
    /// <param name="id">The ID of the service request to attach the quotation to.</param>
    /// <param name="dto">The quotation payload containing line items and optional notes.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The newly created quotation summary including the quotation ID and grand total.</returns>
    /// <response code="201">Quotation created successfully.</response>
    /// <response code="400">Validation failed (e.g. empty items, quantity &lt; 1, negative unit price).</response>
    /// <response code="404">The specified service request was not found.</response>
    /// <response code="422">Business rule violation (e.g. request is already closed or cancelled).</response>
    [HttpPost("{id:guid}/quotations")]
    [ProducesResponseType(typeof(QuotationGeneratedResult), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> GenerateQuotation(
        [FromRoute] Guid id,
        [FromBody] CreateQuotationDto dto,
        CancellationToken ct)
    {
        // FluentValidation auto-validation runs before this body executes.
        // If ModelState is invalid, ASP.NET Core returns 400 automatically.

        var command = new GenerateQuotationCommand(id, dto);
        var result = await _generateQuotationHandler.HandleAsync(command, ct);

        if (!result.IsSuccess)
        {
            // Distinguish "not found" from genuine business-rule violations for accurate HTTP semantics.
            var isNotFound = result.Error?.Contains("was not found", StringComparison.OrdinalIgnoreCase) ?? false;

            return isNotFound
                ? Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound)
                : Problem(detail: result.Error, statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return CreatedAtAction(
            actionName: nameof(GenerateQuotation),
            routeValues: new { id },
            value: result.Value);
    }

    /// <summary>
    /// Updates the status of a service request and records history.
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] Guid id,
        [FromBody] UpdateServiceRequestStatusDto dto,
        CancellationToken ct)
    {
        var command = new UpdateServiceRequestStatusCommand(id, dto);
        var result = await _updateStatusHandler.HandleAsync(command, ct);

        if (!result.IsSuccess)
        {
            if (result.Error!.Contains("was not found", StringComparison.OrdinalIgnoreCase))
                return Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound);

            return Problem(detail: result.Error, statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return Ok(result.Value);
    }
}
