using MediatR;
using Workshop.Application.Features.Workers.DTOs;
using Workshop.Domain.Common;

namespace Workshop.Application.Features.Workers.Queries;

public record GetWorkerCommissionsQuery(Guid WorkerId, int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResult<WorkerCommissionDto>>>;
