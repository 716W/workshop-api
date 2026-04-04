using MediatR;
using Workshop.Domain.Common;

namespace Workshop.Application.Features.ServiceRequests.Commands;

/// <summary>
/// Command to close a service request as successfully completed.
/// Expected to be used when the vehicle is Ready_For_Release.
/// </summary>
public record CloseServiceRequestCommand(Guid ServiceRequestId) : IRequest<Result<bool>>;
