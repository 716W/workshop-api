using MediatR;
using Workshop.Domain.Common;

namespace Workshop.Application.Commands;

public record GenerateInvoiceCommand(Guid ServiceRequestId) : IRequest<Result<Guid>>;
