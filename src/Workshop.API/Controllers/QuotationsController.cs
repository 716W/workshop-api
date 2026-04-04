using Microsoft.AspNetCore.Mvc;

namespace Workshop.API.Controllers;

/// <summary>
/// Handles customer decisions on quotations: approval and rejection.
/// </summary>
[ApiController]
[Route("api/quotations")]
public sealed class QuotationsController : ControllerBase
{
    private readonly ICommandHandler<ApproveQuotationCommand, ApproveQuotationResult> _approveHandler;
    private readonly ICommandHandler<RejectQuotationCommand, RejectQuotationResult> _rejectHandler;

    public QuotationsController(
        ICommandHandler<ApproveQuotationCommand, ApproveQuotationResult> approveHandler,
        ICommandHandler<RejectQuotationCommand, RejectQuotationResult> rejectHandler)
    {
        _approveHandler = approveHandler;
        _rejectHandler = rejectHandler;
    }

    /// <summary>
    /// Approves a quotation on behalf of the customer.
    /// Transitions the linked service request to <c>In_Progress</c> and
    /// triggers inventory allocation (or <c>PurchaseNeed</c> creation for out-of-stock parts).
    /// </summary>
    /// <param name="id">The ID of the quotation to approve.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Quotation approved; work is now in progress.</response>
    /// <response code="404">Quotation not found.</response>
    /// <response code="422">Business rule violation (e.g. quotation is not in Pending state).</response>
    [HttpPost("{id:guid}/approve")]
    [ProducesResponseType(typeof(ApproveQuotationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> ApproveQuotation([FromRoute] Guid id, CancellationToken ct)
    {
        var command = new ApproveQuotationCommand(id);
        var result = await _approveHandler.HandleAsync(command, ct);

        if (!result.IsSuccess)
        {
            var isNotFound = result.Error?.Contains("was not found", StringComparison.OrdinalIgnoreCase) ?? false;
            return isNotFound
                ? Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound)
                : Problem(detail: result.Error, statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Rejects a quotation on behalf of the customer.
    /// Transitions the linked service request to <c>Closed_Rejected</c> and
    /// automatically generates a pending inspection-fee invoice of 150.00.
    /// </summary>
    /// <param name="id">The ID of the quotation to reject.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Quotation rejected; inspection-fee invoice created.</response>
    /// <response code="404">Quotation not found.</response>
    /// <response code="422">Business rule violation (e.g. quotation is not in Pending state).</response>
    [HttpPost("{id:guid}/reject")]
    [ProducesResponseType(typeof(RejectQuotationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RejectQuotation([FromRoute] Guid id, CancellationToken ct)
    {
        var command = new RejectQuotationCommand(id);
        var result = await _rejectHandler.HandleAsync(command, ct);

        if (!result.IsSuccess)
        {
            var isNotFound = result.Error?.Contains("was not found", StringComparison.OrdinalIgnoreCase) ?? false;
            return isNotFound
                ? Problem(detail: result.Error, statusCode: StatusCodes.Status404NotFound)
                : Problem(detail: result.Error, statusCode: StatusCodes.Status422UnprocessableEntity);
        }

        return Ok(result.Value);
    }
}
