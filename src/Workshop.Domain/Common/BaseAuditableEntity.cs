namespace Workshop.Domain.Common;

/// <summary>
/// Base class for all auditable domain entities.
/// Provides a consistent primary key and automatic audit timestamps.
/// </summary>
public abstract class BaseAuditableEntity
{
    /// <summary>Unique identifier (primary key) for the entity.</summary>
    public Guid Id { get; set; }

    /// <summary>UTC timestamp of when the record was first created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>UTC timestamp of the last modification. Null until the entity is first updated.</summary>
    public DateTime? UpdatedAt { get; set; }
}
