using MediatR;
using Workshop.Application.Features.Inventory.DTOs;
using Workshop.Domain.Common;

namespace Workshop.Application.Features.Inventory.Queries;

public record GetPagedPartsQuery(int PageNumber = 1, int PageSize = 10, string? SearchTerm = null) : IRequest<Result<PagedResult<PartDto>>>;
