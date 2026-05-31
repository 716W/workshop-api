using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Workshop.Application.Features.Inventory.DTOs;
using Workshop.Application.Features.Inventory.Queries;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.Inventory.Handlers;

public class GetPartByIdQueryHandler : IRequestHandler<GetPartByIdQuery, Result<PartDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPartByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PartDto>> Handle(GetPartByIdQuery request, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<Part>();

        var part = await repo.GetByIdAsync(request.Id);
        
        if (part == null)
            return Result<PartDto>.Failure("Part not found.");

        var dto = new PartDto(
            Id: part.Id,
            Name: part.Name,
            PartNumber: part.PartNumber,
            Description: part.Description,
            UnitPrice: part.UnitPrice,
            QuantityInStock: part.QuantityInStock,
            ReorderLevel: part.ReorderLevel,
            CreatedAt: part.CreatedAt
        );

        return Result<PartDto>.Success(dto);
    }
}
