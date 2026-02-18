namespace Workshop.Domain.Entities;

public class Mechanic
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Specialization { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation: One Mechanic can have many JobCards
    public ICollection<JobCard> JobCards { get; set; } = new List<JobCard>();
}
