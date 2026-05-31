using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Application.Features.Customers.DTOs;
using Workshop.Application.Features.Vehicles.DTOs;
using Workshop.Application.Features.ServiceRequests.DTOs;
using Workshop.Application.Features.Customers.Queries;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.Customers.Handlers;

public class GetCustomerHistoryQueryHandler : IRequestHandler<GetCustomerHistoryQuery, Result<CustomerHistoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCustomerHistoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CustomerHistoryDto>> Handle(GetCustomerHistoryQuery request, CancellationToken cancellationToken)
    {
        var customerRepo = _unitOfWork.Repository<Customer>();
        var serviceRequestRepo = _unitOfWork.Repository<ServiceRequest>();

        var customers = await customerRepo.FindAsync(
            predicate: c => c.Id == request.CustomerId,
            include: q => q.Include(c => c.Vehicles),
            asNoTracking: true
        );
        var customer = customers.FirstOrDefault();

        if (customer == null)
            return Result<CustomerHistoryDto>.Failure($"Customer with ID {request.CustomerId} not found.");

        var serviceRequests = await serviceRequestRepo.FindAsync(
            predicate: sr => sr.CustomerId == request.CustomerId,
            include: q => q.Include(r => ((RepairRequest)r).Vehicle)
                           .Include(r => ((InspectionRequest)r).Vehicle),
            asNoTracking: true
        );

        var dtos = serviceRequests
            .Select(sr => new ServiceRequestSummaryDto(
                Id: sr.Id,
                Date: sr.Date,
                CustomerName: $"{customer.FirstName} {customer.LastName}".Trim(),
                VehiclePlate: sr is RepairRequest rep ? rep.Vehicle?.PlateNumber ?? "" : (sr is InspectionRequest insp ? insp.Vehicle?.PlateNumber ?? "" : ""),
                RequestType: sr.RequestType,
                Status: sr.Status,
                TotalAmount: sr.Price
            ))
            .OrderByDescending(sr => sr.Date)
            .ToList();

        var historyDto = new CustomerHistoryDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            PhoneNumber = customer.PhoneNumber,
            Email = customer.Email,
            Vehicles = customer.Vehicles.Select(v => new VehicleDto
            {
                Id = v.Id,
                Make = v.Make,
                Model = v.Model,
                Year = v.Year,
                PlateNumber = v.PlateNumber,
                VinNumber = v.VinNumber,
                CustomerId = v.CustomerId
            }).ToList(),
            ServiceRequests = dtos
        };

        return Result<CustomerHistoryDto>.Success(historyDto);
    }
}
