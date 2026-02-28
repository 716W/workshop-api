using Workshop.Domain.Common;

namespace Workshop.Domain.Entities;

public class Mechanic : BaseAuditableEntity
{
    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Specialization { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public bool IsAvailable { get; set; } = true;

    // Navigation: One Mechanic can have many JobCards
    public ICollection<JobCard> JobCards { get; set; } = new List<JobCard>();
}
