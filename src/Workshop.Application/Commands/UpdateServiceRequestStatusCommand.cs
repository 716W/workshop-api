using MediatR;
using Workshop.Application.DTOs;
using Workshop.Domain.Common;

namespace Workshop.Application.Commands;

public record UpdateServiceRequestStatusCommand(
    Guid ServiceRequestId,
    UpdateServiceRequestStatusDto Dto) : IRequest<Result<Guid>>;
