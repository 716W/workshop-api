using MediatR;
using Workshop.Domain.Common;

namespace Workshop.Application.Features.ServiceRequests.Commands;

public record UpdateServiceRequestStatusCommand(
    Guid ServiceRequestId,
    UpdateServiceRequestStatusDto Dto) : IRequest<Result<Guid>>;
