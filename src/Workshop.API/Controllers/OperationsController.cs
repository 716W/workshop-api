using Microsoft.AspNetCore.Mvc;
using Workshop.API.Routes;
using Workshop.Application.Commands;
using Workshop.Application.DTOs;
using Workshop.Application.Interfaces;

namespace Workshop.API.Controllers;

/// <summary>
/// Handles workshop-floor operations such as generating quotations for service requests.
/// </summary>
[Route(ApiRoutes.Operations.Base)]
public sealed class OperationsController : BaseApiController
{
    private readonly ICommandHandler<GenerateQuotationCommand, QuotationGeneratedResult> _generateQuotationHandler;

    public OperationsController(
        ICommandHandler<GenerateQuotationCommand, QuotationGeneratedResult> generateQuotationHandler)
    {
        _generateQuotationHandler = generateQuotationHandler;
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
    /// <response code="400">Validation failed or business rule violation (e.g. request already closed).</response>
    /// <response code="404">The specified service request was not found.</response>
    [HttpPost(ApiRoutes.Operations.GenerateQuotation)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<QuotationGeneratedResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<QuotationGeneratedResult>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<QuotationGeneratedResult>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateQuotation(
        [FromRoute] Guid id,
        [FromBody] CreateQuotationDto dto,
        CancellationToken ct)
    {
        // FluentValidation auto-validation runs before this body executes.

        var command = new GenerateQuotationCommand(id, dto);
        var result  = await _generateQuotationHandler.HandleAsync(command, ct);

        var location = Url.Action(nameof(GenerateQuotation), new { id }) ?? string.Empty;
        return HandleCreated(result, location);
    }
}
