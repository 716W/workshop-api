using Microsoft.AspNetCore.Mvc;
using Workshop.API.Routes;
using Workshop.Application.Features.Quotations.Commands;
using Workshop.Application.Features.Quotations.DTOs;
using Workshop.Application.Features.ServiceRequests.Commands;
using Workshop.Application.Features.ServiceRequests.DTOs;
using Workshop.Application.Features.QC.Commands;
using Workshop.Application.Features.QC.DTOs;
using Workshop.Application.Interfaces;

namespace Workshop.API.Controllers;

/// <summary>
/// Handles workshop-floor operations such as generating quotations for service requests.
/// </summary>
[Route(ApiRoutes.Operations.Base)]
public sealed class OperationsController : BaseApiController
{
    private readonly ICommandHandler<GenerateQuotationCommand, QuotationGeneratedResult> _generateQuotationHandler;
    private readonly ICommandHandler<UpdateServiceRequestStatusCommand, Guid> _updateStatusHandler;
    private readonly ICommandHandler<PerformQCCommand, Guid> _performQCHandler;

    public OperationsController(
        ICommandHandler<GenerateQuotationCommand, QuotationGeneratedResult> generateQuotationHandler,
        ICommandHandler<UpdateServiceRequestStatusCommand, Guid> updateStatusHandler,
        ICommandHandler<PerformQCCommand, Guid> performQCHandler)
    {
        _generateQuotationHandler = generateQuotationHandler;
        _updateStatusHandler = updateStatusHandler;
        _performQCHandler = performQCHandler;
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
        var command = new GenerateQuotationCommand(id, dto);
        var result  = await _generateQuotationHandler.HandleAsync(command, ct);

        var location = Url.Action(nameof(GenerateQuotation), new { id }) ?? string.Empty;
        return HandleCreated(result, location);
    }

    /// <summary>
    /// Updates the status of a service request and records history.
    /// </summary>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        [FromRoute] Guid id,
        [FromBody] UpdateServiceRequestStatusDto dto,
        CancellationToken ct)
    {
        var command = new UpdateServiceRequestStatusCommand(id, dto);
        var result = await _updateStatusHandler.HandleAsync(command, ct);

        return HandleResult(result);
    }

    /// <summary>
    /// Performs Quality Control (QC) for a service request.
    /// </summary>
    [HttpPost("{id:guid}/qc")]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PerformQC(
        [FromRoute] Guid id,
        [FromBody] PerformQCDto dto,
        CancellationToken ct)
    {
        var command = new PerformQCCommand 
        { 
            ServiceRequestId = id, 
            IsPassed = dto.IsPassed, 
            Notes = dto.Notes 
        };

        var result = await _performQCHandler.HandleAsync(command, ct);

        return HandleResult(result);
    }
}
