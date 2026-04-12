using MediatR;
using Workshop.Domain.Common;
using Workshop.Application.Features.ServiceRequests.DTOs;
using Workshop.Domain.Enums;

namespace Workshop.Application.Features.ServiceRequests.Queries;

public record GetPagedServiceRequestsQuery(
    int PageNumber,
    int PageSize,
    ServiceRequestStatus? Status = null
) : IRequest<Result<PagedResult<ServiceRequestSummaryDto>>>;
