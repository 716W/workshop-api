using MediatR;
using Workshop.Domain.Common;

namespace Workshop.Application.Features.Invoicing.Queries;

public record GetInvoiceByIdQuery(Guid InvoiceId) : IRequest<Result<Workshop.Application.Features.Invoicing.DTOs.InvoiceDetailsDto>>;
