namespace Workshop.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; set; }

    public string Make { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public int Year { get; set; }

    public string PlateNumber { get; set; } = string.Empty;

    public string VinNumber { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Foreign Key: belongs to a Customer
    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    // Navigation: One Vehicle can have many JobCards
    public ICollection<JobCard> JobCards { get; set; } = new List<JobCard>();
}
