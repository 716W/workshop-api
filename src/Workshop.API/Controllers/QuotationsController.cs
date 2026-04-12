using Microsoft.AspNetCore.Mvc;
using Workshop.API.Routes;
using Workshop.Application.Commands;
using Workshop.Application.Interfaces;

namespace Workshop.API.Controllers;

/// <summary>
/// Handles customer decisions on quotations: approval and rejection.
/// </summary>
[Route(ApiRoutes.Quotations.Base)]
public sealed class QuotationsController : BaseApiController
{
    private readonly ICommandHandler<ApproveQuotationCommand, ApproveQuotationResult> _approveHandler;
    private readonly ICommandHandler<RejectQuotationCommand,  RejectQuotationResult>  _rejectHandler;

    public QuotationsController(
        ICommandHandler<ApproveQuotationCommand, ApproveQuotationResult> approveHandler,
        ICommandHandler<RejectQuotationCommand,  RejectQuotationResult>  rejectHandler)
    {
        _approveHandler = approveHandler;
        _rejectHandler  = rejectHandler;
    }

    /// <summary>
    /// Approves a quotation on behalf of the customer.
    /// Transitions the linked service request to <c>In_Progress</c> and
    /// triggers inventory allocation (or <c>PurchaseNeed</c> creation for out-of-stock parts).
    /// </summary>
    /// <param name="id">The ID of the quotation to approve.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Quotation approved; work is now in progress.</response>
    /// <response code="400">Business rule violation (e.g. quotation is not in Pending state).</response>
    /// <response code="404">Quotation not found.</response>
    [HttpPost(ApiRoutes.Quotations.Approve)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<ApproveQuotationResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<ApproveQuotationResult>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<ApproveQuotationResult>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ApproveQuotation([FromRoute] Guid id, CancellationToken ct)
    {
        var command = new ApproveQuotationCommand(id);
        var result  = await _approveHandler.HandleAsync(command, ct);
        return HandleResult(result);
    }

    /// <summary>
    /// Rejects a quotation on behalf of the customer.
    /// Transitions the linked service request to <c>Closed_Rejected</c> and
    /// automatically generates a pending inspection-fee invoice of 150.00.
    /// </summary>
    /// <param name="id">The ID of the quotation to reject.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Quotation rejected; inspection-fee invoice created.</response>
    /// <response code="400">Business rule violation (e.g. quotation is not in Pending state).</response>
    /// <response code="404">Quotation not found.</response>
    [HttpPost(ApiRoutes.Quotations.Reject)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<RejectQuotationResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<RejectQuotationResult>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<RejectQuotationResult>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectQuotation([FromRoute] Guid id, CancellationToken ct)
    {
        var command = new RejectQuotationCommand(id);
        var result  = await _rejectHandler.HandleAsync(command, ct);
        return HandleResult(result);
    }
}
