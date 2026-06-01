# 🔧 Workshop Management System API

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![EF Core](https://img.shields.io/badge/EF%20Core-8.0-68217A?logo=nuget&logoColor=white)](https://learn.microsoft.com/en-us/ef/core/)
[![MySQL](https://img.shields.io/badge/MySQL-8.0-4479A1?logo=mysql&logoColor=white)](https://www.mysql.com/)
[![ASP.NET Core Identity](https://img.shields.io/badge/Identity-JWT%20Auth-blue)](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)
[![Architecture](https://img.shields.io/badge/Architecture-Clean%20%2B%20CQRS-brightgreen)](#architecture)
[![License](https://img.shields.io/badge/License-MIT-yellow)](LICENSE)

> A production-ready backend API for managing the full lifecycle of an automotive workshop — from vehicle check-in through inspection, quotation, repair, QC, invoicing, and final release.

---

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Tech Stack](#tech-stack)
- [Modules Overview](#modules-overview)
- [Getting Started](#getting-started)
- [API Endpoints at a Glance](#api-endpoints-at-a-glance)
- [📚 In-Depth Documentation](#-in-depth-documentation)

---

## Overview

Most workshop management tools are either bloated desktop applications or brittle spreadsheets. This API provides a lean, maintainable, and pragmatic backend designed around real workshop operations.

**The business problem it solves:**

- **Receptionists** need a fast way to register vehicles and customers without duplicate data entry.
- **Mechanics** need a clear job board showing what to work on and in what priority.
- **QC Inspectors** need a dedicated gate before any vehicle is handed back to a customer.
- **Accountants** need accurate, auditable invoices tied directly to approved work orders.
- **Managers** need a single source of truth for every vehicle's status, history, and worker commissions.

This API serves all four personas from a single, secured, role-aware backend.

---

## Architecture

This project follows **Pragmatic Clean Architecture** — structured enough to scale, lean enough to avoid over-engineering. The philosophy is: *the architecture serves the business, not the other way around*.

```
┌─────────────────────────────────────────────┐
│               Workshop.API                  │  ← Thin controllers, DI wiring, JWT auth
├─────────────────────────────────────────────┤
│            Workshop.Application             │  ← CQRS (Commands + Queries), DTOs, Validation
├─────────────────────────────────────────────┤
│              Workshop.Domain                │  ← Entities, Enums, Domain Events (no deps)
├─────────────────────────────────────────────┤
│           Workshop.Infrastructure           │  ← EF Core, MySQL, Identity, File Storage
└─────────────────────────────────────────────┘
```

### Key Architectural Patterns

| Pattern | Implementation | Why |
|---|---|---|
| **CQRS** | MediatR `IRequest` / `INotification` | Separates read and write concerns cleanly |
| **Domain Events** | `QuotationApprovedEvent` → `AllocatePartsEventHandler` | Decouples inventory logic from the approval flow |
| **Result Pattern** | `Result<T>` (no exceptions for business failures) | Predictable, type-safe error propagation |
| **Repository + UoW** | Generic `IRepository<T>` + `IUnitOfWork` | Abstracts EF Core from the application layer |
| **Audit Interceptor** | `ISaveChangesInterceptor` + `ICurrentUserService` | Automatic `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy` on every entity |

> **Design principle**: Controllers are thin. They receive a request, dispatch a `Command` or `Query` via MediatR, and return a standardized `ApiResponse<T>`. Zero business logic lives in controllers.

---

## Tech Stack

| Layer | Technology |
|---|---|
| **Runtime** | .NET 8 (C# 12) |
| **Web Framework** | ASP.NET Core 8 Web API |
| **ORM** | Entity Framework Core 8 (Pomelo MySQL provider) |
| **Database** | MySQL 8.0 |
| **Authentication** | ASP.NET Core Identity + JWT Bearer Tokens |
| **Mediator / CQRS** | MediatR 12 |
| **Validation** | FluentValidation (auto-wired via pipeline behavior) |
| **Dependency Injection** | Built-in `IServiceCollection` (manual, no extra framework) |
| **File Storage** | Local filesystem (`wwwroot/uploads`) with `IFileStorageService` abstraction |
| **API Testing** | JetBrains HTTP Client (`.http` files) |

---

## Modules Overview

The system is organized around four business domains, each implemented as a vertical slice of CQRS commands and queries behind role-secured endpoints.

### 🚗 1. Workshop Board (Core Module)
The heart of the system. Manages the full 7-stage vehicle lifecycle:
`Registered → Pending Approval → In Progress → Waiting for Parts → Ready for QC → Ready for Invoicing → Closed`

Key operations: Create Service Request · Generate Quotation · Approve/Reject · Update Status · Perform QC · Upload Damage Photos.

### 📦 2. Inventory Module
Tracks spare parts stock. When a quotation is approved, a Domain Event automatically creates `PurchaseNeed` records for any out-of-stock items, alerting the purchasing team.

Key operations: Manage Parts · View Pending Purchase Needs · Resolve Needs (restock).

### 👥 3. Customers & Vehicles Module
Maintains the customer registry and their vehicle history. Receptionists can search by name or phone number and view a complete service history per vehicle.

Key operations: Search Customers · View Customer History · Update Customer & Vehicle Info.

### 💰 4. Finance & Auth Module
Handles invoicing, payments, and worker commission calculations. Secured by role-based JWT authentication with business-specific roles.

Key operations: Login · Register Worker Account · Generate Invoice · Process Payment · View Invoice Details · View Worker Commissions.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- MySQL 8.0 (running locally or via Docker)
- A tool to call HTTP APIs: [JetBrains HTTP Client](https://www.jetbrains.com/help/idea/http-client-in-product-code-editor.html), Postman, or `curl`

### 1. Clone the Repository

```bash
git clone https://github.com/your-org/workshop-api.git
cd workshop-api
```

### 2. Configure the Database Connection

Open `src/Workshop.API/appsettings.Development.json` and update the MySQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WorkshopDb;User=root;Password=your_password;"
  },
  "JwtSettings": {
    "SecretKey": "your-very-long-secret-key-here-min-32-chars",
    "Issuer": "WorkshopAPI",
    "Audience": "WorkshopClients",
    "ExpirationInMinutes": 60
  }
}
```

### 3. Apply Database Migrations

```bash
cd src/Workshop.API
dotnet ef database update --project ../Workshop.Infrastructure
```

> **Note:** This will create the `WorkshopDb` database and all tables, including seeded roles (`Manager`, `Receptionist`, `Mechanic`, `QC_Inspector`, `Inventory_Manager`, `Accountant`) and a default Manager account.

### 4. Run the API

```bash
dotnet run --project src/Workshop.API
```

The API will start on `https://localhost:5001` (or the port shown in the console). Swagger UI is available at `/swagger`.

### 5. Run the Manual Test Suite

Open `tests/Manual/WorkshopScenarios.http` in VS Code (with the REST Client extension) or JetBrains Rider. The file walks through the complete vehicle lifecycle end-to-end, including authentication.

---

## API Endpoints at a Glance

| Module | Method | Route | Role |
|---|---|---|---|
| Auth | `POST` | `/api/auth/login` | Public |
| Auth | `POST` | `/api/auth/register-worker` | Manager |
| Reception | `POST` | `/api/reception/create` | Receptionist, Manager |
| Reception | `GET` | `/api/reception/requests` | Any Authorized |
| Reception | `GET` | `/api/reception/requests/{id}` | Any Authorized |
| Operations | `POST` | `/api/requests/{id}/quotations` | Mechanic, Manager |
| Operations | `PATCH` | `/api/requests/{id}/status` | Mechanic, Manager |
| Operations | `POST` | `/api/requests/{id}/qc` | QC_Inspector, Manager |
| Operations | `POST` | `/api/requests/{id}/attachments` | Any Authorized |
| Quotations | `POST` | `/api/quotations/{id}/approve` | Manager, Receptionist |
| Quotations | `POST` | `/api/quotations/{id}/reject` | Manager, Receptionist |
| Billing | `POST` | `/api/requests/{id}/invoice` | Accountant, Manager |
| Billing | `POST` | `/api/invoices/{id}/pay` | Accountant, Manager |
| Billing | `GET` | `/api/invoices/{id}` | Accountant, Manager |
| Workers | `GET` | `/api/workers/{id}/commissions` | Manager, Accountant |

---

## 📚 In-Depth Documentation

The `docs/` folder uses a **Docs-as-Code** approach — all documentation lives in the repository and evolves alongside the code.

### Business Flows

| Document | Description |
|---|---|
| [🚗 Workshop Lifecycle & Business Flows](./docs/01-Business-Flows/workshop-lifecycle.md) | The complete 7-stage vehicle lifecycle from reception to release |

### Features

| Document | Description |
|---|---|
| [🔐 Authentication & Identity](./docs/02-Features/authentication.md) | JWT setup, roles, claims, and how `ICurrentUserService` works |
| [📸 Media & Attachments](./docs/02-Features/media-attachments.md) | File upload flow, storage strategy, and `IFileStorageService` |

### Architecture

| Document | Description |
|---|---|
| [🏗️ Clean Architecture Overview](./docs/03-Architecture/clean-architecture.md) | Layer responsibilities, dependency rules, and project structure |
| [⚡ CQRS & MediatR Patterns](./docs/03-Architecture/cqrs-mediatr.md) | How Commands, Queries, and Domain Events are structured |

### Future Ideas

| Document | Description |
|---|---|
| [🚀 Roadmap & Future Ideas](./docs/04-Future-Ideas/roadmap.md) | Planned enhancements and potential feature expansions |

---

## Contributing

1. Follow the Conventional Commits format: `feat(scope): description`
2. One branch per feature or fix (prefix: `feature/`, `fix/`, `refactor/`, etc.)
3. All business logic must live in the Application layer — never in controllers
4. Run `dotnet build` before opening a PR

---

*Built with pragmatic engineering principles — clean where it matters, simple where it can be.*
