using MediatR;
using Microsoft.AspNetCore.Mvc;
using Workshop.Domain.Enums;

namespace Workshop.API.Controllers;

[ApiController]
[Route("api")] // Route prefixes applied on methods to match requirements
public class BillingController(IMediator mediator) : ControllerBase
{
    [HttpPost("requests/{id:guid}/invoice")]
    public async Task<IActionResult> GenerateInvoice(Guid id)
    {
        var command = new GenerateInvoiceCommand(id);
        var result = await mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { InvoiceId = result.Value, Message = "Invoice generated successfully." });

        return BadRequest(new { Error = result.Error });
    }

    [HttpPost("invoices/{id:guid}/pay")]
    public async Task<IActionResult> PayInvoice(Guid id, [FromBody] PayInvoiceRequest request)
    {
        var command = new ProcessPaymentCommand(
            InvoiceId: id,
            Amount: request.Amount,
            PaymentMethod: request.PaymentMethod,
            TransactionReference: request.TransactionReference ?? string.Empty
        );

        var result = await mediator.Send(command);

        if (result.IsSuccess)
            return Ok(new { PaymentId = result.Value, Message = "Payment processed successfully." });

        return BadRequest(new { Error = result.Error });
    }
}

public class PayInvoiceRequest
{
    public decimal Amount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string? TransactionReference { get; set; }
}
