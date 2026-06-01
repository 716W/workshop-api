[⬅️ Back to Main README](../../README.md)

# 🔐 Authentication & Identity

This document covers how authentication and authorization are implemented in the Workshop Management System API.

---

## Overview

Authentication is handled by **ASP.NET Core Identity** backed by MySQL. Once verified, the system issues a **JWT Bearer Token** containing the user's role and `WorkerId` as claims. All protected endpoints validate this token via the standard `[Authorize]` middleware.

---

## Roles

The system uses business-driven roles, seeded automatically on first migration:

| Role | Permissions Summary |
|---|---|
| `Manager` | Full access to all endpoints |
| `Receptionist` | Create service requests, approve/reject quotations, release vehicles |
| `Mechanic` | Generate quotations, update service request status |
| `QC_Inspector` | Perform quality control checks |
| `Inventory_Manager` | Manage parts and resolve purchase needs |
| `Accountant` | Generate invoices, process payments, view financial reports |

---

## JWT Token Structure

The JWT payload includes:

```json
{
  "sub": "user-guid",
  "email": "mechanic@workshop.com",
  "role": "Mechanic",
  "workerId": "worker-guid",
  "exp": 1234567890
}
```

The `workerId` claim allows handlers and interceptors to automatically attribute database records to the correct workshop employee without any extra API parameters.

---

## Current User Service

The `ICurrentUserService` interface (defined in `Workshop.Application`) provides a clean abstraction for reading identity claims from anywhere in the Application or Infrastructure layer:

```csharp
public interface ICurrentUserService
{
    string? GetUserId();
    Guid? GetWorkerId();
    string? GetUserRole();
}
```

The implementation in `Workshop.Infrastructure` reads directly from `IHttpContextAccessor`, keeping the Application layer free of HTTP dependencies.

---

## Audit Integration

The EF Core `ISaveChangesInterceptor` uses `ICurrentUserService` to automatically populate:

- `CreatedBy` — on entity insert
- `UpdatedBy` — on entity update

This ensures every write operation in the system is traceable to the authenticated user, with no manual effort required in command handlers.

---

## Auth Endpoints

| Method | Route | Access | Description |
|---|---|---|---|
| `POST` | `/api/auth/login` | Public | Returns JWT on valid credentials |
| `POST` | `/api/auth/register-worker` | Manager | Creates an `ApplicationUser` linked to an existing `Worker` entity |

---

## Security Notes

- Passwords are hashed by ASP.NET Core Identity (PBKDF2 by default).
- JWTs are signed with HMACSHA256. The secret key must be at least 32 characters and stored securely (use environment variables in production, never `appsettings.json`).
- All non-auth endpoints require a valid JWT. Unauthorized requests receive `401 Unauthorized`.
- Endpoints with role restrictions return `403 Forbidden` for users with insufficient privileges.
