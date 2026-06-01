[⬅️ Back to Main README](../../README.md)

# ⚡ CQRS & MediatR Patterns

This document explains how Commands, Queries, and Domain Events are structured and flow through the system.

---

## Why CQRS?

**Command Query Responsibility Segregation** separates write operations (Commands) from read operations (Queries). In a workshop system this matters because:

- **Reads** (e.g., "show me the board") need different projections and join strategies than **writes** (e.g., "approve this quotation").
- Read models can be optimized with `AsNoTracking()` and direct projections to DTOs, without loading full entity graphs.
- Write handlers enforce business rules; read handlers focus on data shape.

---

## MediatR Pipeline

All Commands and Queries are dispatched through MediatR's `IMediator.Send()`. The pipeline:

```
Controller.Send(command)
    │
    ▼
[ValidationBehavior]       ← FluentValidation runs here (auto-registered)
    │
    ▼
[Handler.Handle()]         ← Business logic executes
    │
    ▼
Result<T> returned
    │
    ▼
Controller maps to HTTP response
```

---

## Command Structure

Each Command follows a consistent pattern:

```csharp
// 1. The Command (record, immutable)
public record ApproveQuotationCommand(Guid QuotationId) : IRequest<Result<Guid>>;

// 2. The Handler
public class ApproveQuotationCommandHandler : IRequestHandler<ApproveQuotationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(ApproveQuotationCommand request, CancellationToken ct)
    {
        // Fetch → Validate business rule → Mutate entity → Publish event → Save → Return result
    }
}
```

**Naming convention**: `{Verb}{Entity}Command` — e.g., `GenerateQuotationCommand`, `ProcessPaymentCommand`, `CloseServiceRequestCommand`.

---

## Query Structure

Queries follow the same pattern but use `AsNoTracking()` and project directly to DTOs:

```csharp
// 1. The Query
public record GetServiceRequestByIdQuery(Guid Id) : IRequest<Result<ServiceRequestDetailsDto>>;

// 2. The Handler
public class GetServiceRequestByIdQueryHandler : IRequestHandler<...>
{
    public async Task<Result<ServiceRequestDetailsDto>> Handle(...)
    {
        var entity = await _context.ServiceRequests
            .AsNoTracking()
            .Include(r => r.Quotation).ThenInclude(q => q.Items)
            .Include(r => r.StatusHistory)
            .Include(r => r.Invoice)
            .FirstOrDefaultAsync(r => r.Id == request.Id, ct);

        if (entity is null) return Result<ServiceRequestDetailsDto>.Failure("Not found.");

        return Result<ServiceRequestDetailsDto>.Success(entity.ToDetailsDto());
    }
}
```

**Naming convention**: `Get{Entity}By{Key}Query` or `GetPaged{Entities}Query`.

---

## Domain Events

Domain Events represent significant things that **have happened** in the domain. They are published after a state change and handled asynchronously by one or more handlers.

### Example Flow: Quotation Approval

```
ApproveQuotationCommandHandler
    │
    │  entity.Approve(); // marks quotation as Approved
    │  await _uow.SaveChangesAsync();
    │
    └─► await _mediator.Publish(new QuotationApprovedEvent(quotation.Id));
                                        │
                                        ▼
                          AllocatePartsEventHandler
                              │
                              └─► Creates PurchaseNeed records
                                  for out-of-stock items
```

This decoupling means the approval handler has no knowledge of inventory. Adding a second handler (e.g., to send an SMS notification) requires zero changes to the approval logic.

### Domain Event Definition

```csharp
// In Workshop.Domain/Events/
public record QuotationApprovedEvent(Guid QuotationId) : INotification;
```

---

## Registered Features

| Feature Folder | Commands | Queries |
|---|---|---|
| `ServiceRequests` | Create, UpdateStatus, Close, UploadAttachment | GetPaged, GetById |
| `Quotations` | Generate, Approve, Reject | — |
| `Invoicing` | GenerateInvoice, ProcessPayment | GetInvoiceById |
| `QC` | PerformQC | — |
| `Workers` | — | GetWorkerCommissions |
| `Auth` | Login, RegisterWorker | — |
