using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Application.Features.Customers.DTOs;
using Workshop.Application.Features.Customers.Queries;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.Customers.Handlers;

public class SearchCustomersQueryHandler : IRequestHandler<SearchCustomersQuery, Result<PagedResult<CustomerDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public SearchCustomersQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<PagedResult<CustomerDto>>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        var repo = _unitOfWork.Repository<Customer>();

        var pagedCustomers = await repo.GetPagedAsync(
            pageNumber: request.PageNumber,
            pageSize: request.PageSize,
            predicate: string.IsNullOrWhiteSpace(request.SearchTerm) ? null : 
                c => c.FirstName.Contains(request.SearchTerm) || 
                     c.LastName.Contains(request.SearchTerm) || 
                     c.PhoneNumber.Contains(request.SearchTerm),
            asNoTracking: true
        );

        var dtos = pagedCustomers.Items.Select(c => new CustomerDto
        {
            Id = c.Id,
            FirstName = c.FirstName,
            LastName = c.LastName,
            PhoneNumber = c.PhoneNumber,
            Email = c.Email
        }).OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();

        var pagedDto = new PagedResult<CustomerDto>(
            Items: dtos,
            TotalCount: pagedCustomers.TotalCount,
            PageNumber: pagedCustomers.PageNumber,
            PageSize: pagedCustomers.PageSize
        );

        return Result<PagedResult<CustomerDto>>.Success(pagedDto);
    }
}
