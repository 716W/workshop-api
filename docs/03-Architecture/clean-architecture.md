[⬅️ Back to Main README](../../README.md)

# 🏗️ Clean Architecture Overview

This document describes the layer responsibilities, dependency rules, and project structure of the Workshop Management System.

---

## Philosophy

The architecture follows **Pragmatic Clean Architecture** — the goal is maintainability and testability, not architectural purity for its own sake. Every structural decision is justified by a concrete engineering benefit.

> "Make the change easy, then make the easy change." — Kent Beck

---

## Layer Responsibilities

```
┌──────────────────────────────────────────────────────────────┐
│                        Workshop.API                          │
│  • ASP.NET Core 8 Web API                                    │
│  • Thin controllers — dispatch via MediatR, return results   │
│  • JWT middleware, global exception handler                   │
│  • DI wiring (Program.cs)                                    │
└────────────────────────┬─────────────────────────────────────┘
                         │ depends on
┌────────────────────────▼─────────────────────────────────────┐
│                   Workshop.Application                        │
│  • CQRS: Commands, Queries, Handlers                         │
│  • DTOs (request/response shapes)                            │
│  • FluentValidation validators                               │
│  • Interfaces: IRepository<T>, IUnitOfWork,                  │
│    ICurrentUserService, IFileStorageService                  │
│  • Domain Event Handlers (INotificationHandler<>)            │
└────────────────────────┬─────────────────────────────────────┘
                         │ depends on
┌────────────────────────▼─────────────────────────────────────┐
│                     Workshop.Domain                           │
│  • Entities (private setters, behaviour methods)             │
│  • Enums (RequestType, CommissionType, ServiceStatus…)       │
│  • Domain Events (INotification from MediatR)                │
│  • Value Objects (if any)                                    │
│  • NO external package dependencies                          │
└────────────────────────┬─────────────────────────────────────┘
                         │ implemented by
┌────────────────────────▼─────────────────────────────────────┐
│                  Workshop.Infrastructure                      │
│  • EF Core DbContext + Fluent API configurations             │
│  • MySQL via Pomelo provider                                 │
│  • Generic Repository<T> + UnitOfWork                        │
│  • ASP.NET Core Identity                                     │
│  • AuditableEntityInterceptor (ISaveChangesInterceptor)      │
│  • LocalFileStorageService                                   │
│  • Database migrations                                        │
└──────────────────────────────────────────────────────────────┘
```

---

## Dependency Rule

> **Dependencies only point inward.** No inner layer knows about an outer layer.

| Layer | Can depend on |
|---|---|
| Domain | Nothing |
| Application | Domain only |
| Infrastructure | Domain + Application (for interface implementations) |
| API | Application + Infrastructure (for DI registration) |

---

## Project Structure

```
workshop-api/
├── src/
│   ├── Workshop.Domain/
│   │   ├── Entities/           # ServiceRequest, Quotation, Invoice, …
│   │   ├── Enums/              # RequestType, ServiceStatus, …
│   │   └── Events/             # QuotationApprovedEvent, …
│   │
│   ├── Workshop.Application/
│   │   ├── Features/
│   │   │   ├── ServiceRequests/   # Commands + Queries + Handlers
│   │   │   ├── Quotations/
│   │   │   ├── Invoicing/
│   │   │   └── QC/
│   │   ├── Interfaces/            # IRepository, IUnitOfWork, ICurrentUserService
│   │   └── Common/                # Result<T>, PagedResult<T>, base DTOs
│   │
│   ├── Workshop.Infrastructure/
│   │   ├── Persistence/
│   │   │   ├── WorkshopDbContext.cs
│   │   │   ├── Configurations/    # IEntityTypeConfiguration<T> per entity
│   │   │   └── Migrations/
│   │   ├── Repositories/          # GenericRepository<T>, UnitOfWork
│   │   └── Services/              # LocalFileStorageService, AuthService
│   │
│   └── Workshop.API/
│       ├── Controllers/           # Thin controllers, inherit BaseApiController
│       ├── Contracts/             # ApiResponse<T>, PagedResponse<T>
│       ├── Routes/                # ApiRoutes static class (all route constants)
│       └── Program.cs
│
├── tests/
│   └── Manual/
│       └── WorkshopScenarios.http  # End-to-end HTTP test suite
│
└── docs/                           # This documentation
```

---

## Key Design Decisions

### Result Pattern over Exceptions

Business failures (e.g., "quotation already approved") are returned as `Result<T>` with a typed failure message — not as thrown exceptions. This makes failure paths explicit and testable.

```csharp
// Handler returns:
return Result<Guid>.Failure("Quotation is already approved.");

// Controller maps:
return HandleResult(result); // → 400 BadRequest with ApiResponse
```

### Audit Interceptor

Rather than requiring every command handler to manually set `CreatedAt`, `UpdatedAt`, `CreatedBy`, and `UpdatedBy`, a single `ISaveChangesInterceptor` intercepts all save operations and populates these fields automatically using `ICurrentUserService`.

### Standardized API Responses

All endpoints return `ApiResponse<T>` (or `PagedResponse<T>` for lists). The `BaseApiController` provides `HandleResult()`, `HandleCreated()`, and `HandlePagedResult()` helpers that map `Result<T>` to the appropriate HTTP response — eliminating repetitive `if (!result.IsSuccess)` branches in controllers.
