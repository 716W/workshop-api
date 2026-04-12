using System;
using MediatR;
using Workshop.Application.Features.Inventory.DTOs;
using Workshop.Domain.Common;

namespace Workshop.Application.Features.Inventory.Queries;

public record GetPartByIdQuery(Guid Id) : IRequest<Result<PartDto>>;
