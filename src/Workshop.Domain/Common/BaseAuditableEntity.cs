namespace Workshop.Domain.Common;

/// <summary>
/// Base class for all auditable domain entities.
/// Provides a consistent primary key, automatic audit timestamps,
/// and the Identity (user ID) of the actor who created or last modified the record.
/// </summary>
public abstract class BaseAuditableEntity
{
    /// <summary>Unique identifier (primary key) for the entity.</summary>
    public Guid Id { get; set; }

    /// <summary>UTC timestamp of when the record was first created.</summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>UTC timestamp of the last modification. Null until the entity is first updated.</summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// The ID of the authenticated user who created this record.
    /// Null for records created by anonymous/system operations.
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// The ID of the authenticated user who last modified this record.
    /// Null until the record is first updated by an authenticated user.
    /// </summary>
    public string? UpdatedBy { get; set; }
}
