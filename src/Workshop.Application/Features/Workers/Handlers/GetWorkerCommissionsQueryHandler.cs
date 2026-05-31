using MediatR;
using Microsoft.EntityFrameworkCore;
using Workshop.Application.Features.Workers.DTOs;
using Workshop.Application.Features.Workers.Queries;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.Workers.Handlers;

public class GetWorkerCommissionsQueryHandler : IRequestHandler<GetWorkerCommissionsQuery, Result<PagedResult<WorkerCommissionDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetWorkerCommissionsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<WorkerCommissionDto>>> Handle(GetWorkerCommissionsQuery request, CancellationToken cancellationToken)
    {
        var pagedCommissions = await _unitOfWork.Repository<WorkerCommission>().GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            predicate: wc => wc.WorkerId == request.WorkerId,
            asNoTracking: true
        );

        var dtos = pagedCommissions.Items.Select(wc => new WorkerCommissionDto(
            wc.Id,
            wc.Amount,
            wc.CreatedAt,
            wc.ServiceRequestId
        )).ToList();

        var pagedDto = new PagedResult<WorkerCommissionDto>(
            Items: dtos,
            TotalCount: pagedCommissions.TotalCount,
            PageNumber: pagedCommissions.PageNumber,
            PageSize: pagedCommissions.PageSize
        );

        return Result<PagedResult<WorkerCommissionDto>>.Success(pagedDto);
    }
}
