using Workshop.Domain.Enums;

namespace Workshop.Domain.Entities;

/// <summary>
/// The core entity of the Workshop system.
/// Tracks the entire lifecycle of a vehicle repair job.
/// </summary>
public class JobCard
{
    public Guid Id { get; set; }

    public string JobNumber { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string? InspectionNotes { get; set; }

    public JobCardStatus Status { get; set; } = JobCardStatus.CheckedIn;

    public DateTime CheckedInAt { get; set; } = DateTime.UtcNow;

    public DateTime? CompletedAt { get; set; }

    public decimal EstimatedCost { get; set; }

    public decimal? FinalCost { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // FK → Vehicle
    public Guid VehicleId { get; set; }
    public Vehicle Vehicle { get; set; } = null!;

    // FK → Mechanic (assigned)
    public Guid? MechanicId { get; set; }
    public Mechanic? Mechanic { get; set; }

    // Navigation: Parts consumed on this job
    public ICollection<JobCardPart> JobCardParts { get; set; } = new List<JobCardPart>();

    // Navigation: Invoice generated from this job
    public Invoice? Invoice { get; set; }
}
