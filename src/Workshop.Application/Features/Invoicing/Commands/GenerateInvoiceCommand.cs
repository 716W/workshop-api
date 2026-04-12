using MediatR;
using Workshop.Domain.Common;

namespace Workshop.Application.Features.Invoicing.Commands;

public record GenerateInvoiceCommand(Guid ServiceRequestId) : IRequest<Result<Guid>>;
