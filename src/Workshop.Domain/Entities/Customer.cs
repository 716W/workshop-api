using Workshop.Domain.Common;

namespace Workshop.Domain.Entities;

public class Customer : BaseAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    // Navigation: One Customer has many Vehicles
    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
}
