using MediatR;
using Microsoft.AspNetCore.Mvc;
using Workshop.API.Routes;
using Workshop.Application.Features.Invoicing.Commands;
using Workshop.Domain.Enums;

namespace Workshop.API.Controllers;

[Route(ApiRoutes.Billing.Base)]
public class BillingController : BaseApiController
{
    private readonly IMediator _mediator;

    public BillingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost(ApiRoutes.Billing.GenerateInvoice)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Contracts.ApiResponse<Guid>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateInvoice(Guid id)
    {
        var command = new GenerateInvoiceCommand(id);
        var result = await _mediator.Send(command);

        return HandleResult(result);
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
}

public class PayInvoiceRequest
{
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? TransactionReference { get; set; }
}
