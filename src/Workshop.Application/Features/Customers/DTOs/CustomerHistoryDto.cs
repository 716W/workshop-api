using Workshop.Application.Features.Vehicles.DTOs;
using Workshop.Application.Features.ServiceRequests.DTOs;

namespace Workshop.Application.Features.Customers.DTOs;

public class CustomerHistoryDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    public List<VehicleDto> Vehicles { get; set; } = new();
    public List<ServiceRequestSummaryDto> ServiceRequests { get; set; } = new();
}
