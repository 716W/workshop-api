using MediatR;
using Workshop.Domain.Common;
using Workshop.Domain.Enums;

namespace Workshop.Application.Commands;

public record ProcessPaymentCommand(
    Guid InvoiceId,
    decimal Amount,
    PaymentMethod PaymentMethod,
    string TransactionReference = ""
) : IRequest<Result<Guid>>;
