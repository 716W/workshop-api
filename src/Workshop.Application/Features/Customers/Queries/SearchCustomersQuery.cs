using MediatR;
using Workshop.Domain.Common;
using Workshop.Application.Features.Customers.DTOs;

namespace Workshop.Application.Features.Customers.Queries;

public class SearchCustomersQuery : IRequest<Result<PagedResult<CustomerDto>>>
{
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
