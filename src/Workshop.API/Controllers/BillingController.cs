using MediatR;
using Microsoft.AspNetCore.Mvc;
using Workshop.API.Routes;
using Workshop.Application.Features.Invoicing.Commands;
using Workshop.Domain.Enums;

namespace Workshop.API.Controllers;

[Route(ApiRoutes.Billing.Base)]
public sealed class BillingController : BaseApiController
{
    private readonly IMediator _mediator;

    public BillingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Generates an invoice for a service request that has passed QC.
    /// Transitions the service request status to <c>Pending_Payment</c>.
    /// </summary>
    /// <response code="201">Invoice created. Location header points to the new invoice resource.</response>
    /// <response code="400">Business rule violation (e.g. request not in Ready_For_Invoicing state).</response>
    /// <response code="404">Service request not found.</response>
    [HttpPost(ApiRoutes.Billing.GenerateInvoice)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateInvoice(Guid id)
    {
        var command = new GenerateInvoiceCommand(id);
        var result = await _mediator.Send(command);

        // Build a Location URI pointing to the newly created invoice resource.
        var location = result.IsSuccess
            ? Url.Action(nameof(GetInvoice), new { id = result.Value }) ?? string.Empty
            : string.Empty;

        return HandleCreated(result, location);
    }

    [HttpPost(ApiRoutes.Billing.PayInvoice)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PayInvoice(Guid id, [FromBody] PayInvoiceRequest request)
    {
        var command = new ProcessPaymentCommand(
            InvoiceId: id,
            Amount: request.Amount,
            PaymentMethod: request.PaymentMethod,
            TransactionReference: request.TransactionReference ?? string.Empty
        );

        var result = await _mediator.Send(command);

        return HandleResult(result);
    }

    [HttpGet(ApiRoutes.Billing.GetById)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Application.Features.Invoicing.DTOs.InvoiceDetailsDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Contracts.ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contracts.ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoice(Guid id)
    {
        var query = new Workshop.Application.Features.Invoicing.Queries.GetInvoiceByIdQuery(id);
        var result = await _mediator.Send(query);

        return HandleResult(result);
    }
}

public class PayInvoiceRequest
{
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? TransactionReference { get; set; }
}
