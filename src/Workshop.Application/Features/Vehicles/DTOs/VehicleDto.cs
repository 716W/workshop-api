namespace Workshop.Application.Features.Vehicles.DTOs;

public class VehicleDto
{
    public Guid Id { get; set; }
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string VinNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
}
