using MediatR;
using Workshop.Domain.Common;
using Workshop.Application.Features.Customers.DTOs;

namespace Workshop.Application.Features.Customers.Queries;

public class GetCustomerHistoryQuery : IRequest<Result<CustomerHistoryDto>>
{
    public Guid CustomerId { get; set; }

    public GetCustomerHistoryQuery(Guid customerId)
    {
        CustomerId = customerId;
    }
}
