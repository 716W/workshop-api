using System;
using MediatR;
using Workshop.Domain.Common;
using Workshop.Application.Features.ServiceRequests.DTOs;

namespace Workshop.Application.Features.ServiceRequests.Queries;

public record GetServiceRequestByIdQuery(Guid Id) : IRequest<Result<ServiceRequestDetailsDto>>;
