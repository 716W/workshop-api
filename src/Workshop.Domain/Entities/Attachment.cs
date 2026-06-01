using Workshop.Domain.Common;

namespace Workshop.Domain.Entities;

/// <summary>
/// Represents a file attachment (e.g. vehicle damage photo) associated with a
/// <see cref="ServiceRequest"/>. Stores metadata only; the actual binary is
/// persisted on disk by <c>IFileStorageService</c>.
/// </summary>
public sealed class Attachment : BaseAuditableEntity
{
    // ── EF Core requires a parameterless constructor ──────────────────────────
    private Attachment() { }

    public Attachment(
        Guid serviceRequestId,
        string fileName,
        string filePath,
        string contentType,
        string? uploadedBy)
    {
        Id               = Guid.NewGuid();
        ServiceRequestId = serviceRequestId;
        FileName         = fileName;
        FilePath         = filePath;
        ContentType      = contentType;
        UploadedAt       = DateTime.UtcNow;
        UploadedBy       = uploadedBy;
    }

    /// <summary>Foreign key to the owning <see cref="ServiceRequest"/>.</summary>
    public Guid ServiceRequestId { get; private set; }

    /// <summary>Original file name as provided by the client (sanitised).</summary>
    public string FileName { get; private set; } = string.Empty;

    /// <summary>
    /// Relative server path where the file is stored (e.g. <c>uploads/abc123_photo.jpg</c>).
    /// Combine with base URL to produce an accessible URI.
    /// </summary>
    public string FilePath { get; private set; } = string.Empty;

    /// <summary>MIME type of the uploaded file (e.g. <c>image/jpeg</c>).</summary>
    public string ContentType { get; private set; } = string.Empty;

    /// <summary>UTC timestamp of when the file was uploaded.</summary>
    public DateTime UploadedAt { get; private set; }

    /// <summary>
    /// The user ID (from JWT) who uploaded the file.
    /// Null if the upload was performed by an anonymous/system operation.
    /// </summary>
    public string? UploadedBy { get; private set; }

    // ── Navigation property ───────────────────────────────────────────────────
    public ServiceRequest ServiceRequest { get; private set; } = null!;
}
