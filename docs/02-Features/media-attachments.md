[⬅️ Back to Main README](../../README.md)

# 📸 Media & Attachments

This document describes the vehicle damage photo management system, including upload flow, storage strategy, and the domain model.

---

## Purpose

When a vehicle enters the workshop, receptionists and mechanics may need to document pre-existing damage or repair progress with photos. The attachments system provides a lightweight, file-based storage mechanism with full database traceability.

---

## Domain Entity

```csharp
public class Attachment : BaseAuditableEntity
{
    public Guid ServiceRequestId { get; private set; }
    public string FileName { get; private set; }
    public string FilePath { get; private set; }
    public string ContentType { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public string UploadedBy { get; private set; }
}
```

Each `Attachment` is linked to a `ServiceRequest`, ensuring all media is fully contextualized within a workshop job.

---

## Upload Flow

```
Client (multipart/form-data)
        │
        ▼
POST /api/requests/{id}/attachments
        │
        ▼
  OperationsController
        │  Dispatches UploadAttachmentCommand
        ▼
    MediatR Handler
        │  1. Validates ServiceRequest exists
        │  2. Calls IFileStorageService.SaveAsync(file)
        │  3. Creates Attachment entity
        │  4. Saves via IUnitOfWork
        ▼
  Returns: { fileUrl, attachmentId }
```

---

## Storage Strategy

The current implementation uses **local filesystem storage** via `LocalFileStorageService`:

- Files are saved under `wwwroot/uploads/`
- A unique GUID prefix is added to every filename to prevent collisions
- The returned URL is relative to the API host (e.g., `/uploads/abc123_damage-front.jpg`)

### `IFileStorageService` Interface

```csharp
public interface IFileStorageService
{
    Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken);
}
```

This abstraction allows swapping the local implementation for cloud storage (e.g., Azure Blob Storage, AWS S3) without touching any application or domain code.

---

## API Endpoint

| Method | Route | Role | Content-Type |
|---|---|---|---|
| `POST` | `/api/requests/{id}/attachments` | Any Authorized | `multipart/form-data` |

### Example Request (HTTP Client)

```http
POST https://localhost:5001/api/requests/{{serviceRequestId}}/attachments
Authorization: Bearer {{authToken}}
Content-Type: multipart/form-data; boundary=boundary

--boundary
Content-Disposition: form-data; name="file"; filename="front-damage.jpg"
Content-Type: image/jpeg

< ./front-damage.jpg
--boundary--
```

---

## Future Improvements

- Cloud storage backend (Azure Blob / S3) via a swapped `IFileStorageService` implementation
- Image compression and thumbnail generation on upload
- Virus/malware scanning before persistence
- Signed URL generation for secure, time-limited file access
