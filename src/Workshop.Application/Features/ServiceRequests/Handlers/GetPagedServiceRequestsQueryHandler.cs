using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Workshop.Application.Features.ServiceRequests.DTOs;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.ServiceRequests.Queries;

public class GetPagedServiceRequestsQueryHandler : IRequestHandler<GetPagedServiceRequestsQuery, Result<PagedResult<ServiceRequestSummaryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPagedServiceRequestsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<ServiceRequestSummaryDto>>> Handle(GetPagedServiceRequestsQuery request, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<ServiceRequest>();

        var pagedRequests = await repo.GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            predicate: request.Status.HasValue ? r => r.Status == request.Status.Value : null,
            include: q => q.Include(r => r.Customer)
                           .Include(r => ((RepairRequest)r).Vehicle)
                           .Include(r => ((InspectionRequest)r).Vehicle),
            asNoTracking: true
        );

        var dtos = pagedRequests.Items.Select(r => new ServiceRequestSummaryDto(
            Id: r.Id,
            Date: r.Date,
            CustomerName: r.Customer != null ? $"{r.Customer.FirstName} {r.Customer.LastName}".Trim() : "Unknown",
            VehiclePlate: r is RepairRequest rep ? rep.Vehicle?.PlateNumber ?? "" : (r is InspectionRequest insp ? insp.Vehicle?.PlateNumber ?? "" : ""),
            RequestType: r.RequestType,
            Status: r.Status,
            TotalAmount: r.Price
        )).ToList();

        var pagedDto = new PagedResult<ServiceRequestSummaryDto>(
            Items: dtos,
            TotalCount: pagedRequests.TotalCount,
            PageNumber: pagedRequests.PageNumber,
            PageSize: pagedRequests.PageSize
        );

        return Result<PagedResult<ServiceRequestSummaryDto>>.Success(pagedDto);
    }
}
