using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Workshop.Application.Features.Inventory.DTOs;
using Workshop.Application.Features.Inventory.Queries;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.Inventory.Handlers;

public class GetPagedPartsQueryHandler : IRequestHandler<GetPagedPartsQuery, Result<PagedResult<PartDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPagedPartsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<PartDto>>> Handle(GetPagedPartsQuery request, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<Part>();

        var pagedParts = await repo.GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            predicate: string.IsNullOrWhiteSpace(request.SearchTerm) ? null : p => p.Name.Contains(request.SearchTerm) || p.PartNumber.Contains(request.SearchTerm),
            asNoTracking: true
        );

        var dtos = pagedParts.Items.Select(p => new PartDto(
            Id: p.Id,
            Name: p.Name,
            PartNumber: p.PartNumber,
            Description: p.Description,
            UnitPrice: p.UnitPrice,
            QuantityInStock: p.QuantityInStock,
            ReorderLevel: p.ReorderLevel,
            CreatedAt: p.CreatedAt
        )).ToList();

        var pagedDto = new PagedResult<PartDto>(
            Items: dtos,
            TotalCount: pagedParts.TotalCount,
            PageNumber: pagedParts.PageNumber,
            PageSize: pagedParts.PageSize
        );

        return Result<PagedResult<PartDto>>.Success(pagedDto);
    }
}
