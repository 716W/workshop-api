# Workshop System Development Plan

Current Status: Phase 5 (Infrastructure Wiring & MySQL Integration) COMPLETE ✅

## Rules for AI Agent

1. Read this file BEFORE doing anything.
2. Only work on tasks marked as [ ] inside the "Current Phase".
3. Do NOT jump to future phases.
4. When a task is done, mark it [x] and update "Current Status".

## Phase 1: Foundation (Dependencies: None)

- [x] **Setup Solution**: Create .NET 8 Web API with Clean Architecture (Domain, Application, Infrastructure, API).
- [x] **Setup Git**: Initialize git and create .gitignore.
- [x] **Setup Database**: Configure SQL Server connection string in appsettings.json.

## Phase 1.5: Architecture Best Practices (Dependencies: Phase 1)

- [x] **Global Exception Handling**: Implement .NET 8 IExceptionHandler in the API layer to catch exceptions globally and return standardized ProblemDetails responses.
- [x] **Audit Tracking (Change Tracker)**: Create a BaseAuditableEntity (with CreatedAt, UpdatedAt, etc.) in the Domain layer. Then, implement an EF Core ISaveChangesInterceptor in the Infrastructure layer to update these properties automatically.
- [x] **Entity Configurations**: Set up a clean structure for EF Core Fluent API by preparing a base or example using IEntityTypeConfiguration<T> in the Infrastructure layer.
- [x] **Advanced Generic Repository**: Upgrade the IGenericRepository and its implementation to support Include (for navigation properties), AsNoTracking (for read-only queries), and basic Pagination.

## Phase 2 (OLD - SUPERSEDED): Domain Entities (Dependencies: Phase 1)

- [x] **Create Customer & Vehicle**: Define entities and relationship (1:N).
- [x] **Create Mechanic & Part**: Define entities.
- [x] **Create JobCard**: The core entity linking Vehicle, Mechanic, and Status.
- [x] **EF Core Context**: Register all DbSets and configurations.

## Phase 2: Scenario 1 - Entry & Service Request Creation

- [x] **Enums & Value Objects**: Create RequestType Enum (Repair, PurchaseOnly, InspectionOnly). Create CommissionType Enum (Fixed, Percentage) in the Domain layer.
- [x] **Base Entity & Polymorphism**: Create a base ServiceRequest entity (containing Price, Date, CustomerId, WorkerId, CommissionType, CommissionValue). Then create derived entities if necessary (e.g., RepairRequest, PurchaseRequest) or handle it via a discriminated discriminator in EF Core.
- [x] **Factory Pattern Implementation**: Implement IServiceRequestFactory and its concrete class. It should take a DTO and return the correct instantiated ServiceRequest object based on the RequestType.
- [x] **DTOs & Validators**: Create CreateServiceRequestDto with FluentValidation. Ensure validation logic adapts to the request type (e.g., VehicleId is required for Repair, but might be optional for PurchaseOnly).
- [x] **CQRS Command**: Create CreateServiceRequestCommand and its Handler. The handler MUST use the Factory to create the entity, save it via the generic repository, and return a Success Result.
- [x] **API Controller**: Create POST /api/requests/create in the ReceptionController.

## Phase 3: Core Logic (Dependencies: Phase 2)

- [x] **Repository Pattern**: Implement Generic Repository & Unit of Work.
- [x] **JobCard Service**: Implement "Check-in" and "Inspection" logic.
- [x] **Inventory Service**: Implement "Consume Part" logic.

## Phase 4: API & Exposure (Dependencies: Phase 3)

- [x] **Controllers**: Create Endpoints for JobCards.
- [x] **DTOs & Validation**: Use FluentValidation.

## 📋 Phase 3: Scenario 2 - Inspection & Quotation Generation

- [x] **Domain Entities**: Create `Quotation` and `QuotationItem` entities. A `Quotation` belongs to a `ServiceRequest`. A `QuotationItem` has a `Type` (Enum: Part or Labor), `Description`, `Quantity`, `UnitPrice`, and `TotalPrice`.
- [x] **Domain Logic**: Add a behavior (method) in `ServiceRequest` to attach a `Quotation` and transition its status to `Pending_Customer_Approval`. Ensure quotations cannot be added to closed or cancelled requests.
- [x] **DTOs & Validators**: Create `CreateQuotationDto` and `QuotationItemDto`. Use FluentValidation to ensure `Quantity` >= 1, `UnitPrice` >= 0, and the list of items is not empty.
- [x] **CQRS Command**: Create `GenerateQuotationCommand` and its Handler. The handler must: retrieve the ServiceRequest, create the Quotation, calculate total costs, update the request status, and save changes via the repository. Return a Success `Result<Guid>` with the Quotation ID.
- [x] **API Controller**: Add a `POST /api/requests/{id}/quotations` endpoint in `OperationsController` to trigger this command.

## 🔄 Phase 4: Scenario 3 - Approval & Inventory Integration

- [x] **Domain Updates**: Add `QuotationStatus` enum (Pending, Approved, Rejected). Add `Status` to `Quotation`. Create `PurchaseNeed` entity (PartName, Quantity, ServiceRequestId, DateRequested). Extend `ServiceRequestStatus` with `In_Progress` and `Closed_Rejected`. Create Domain Event `QuotationApprovedEvent` (implements `INotification`). Add `ApproveQuotation()` and `RejectQuotation()` behaviours to `ServiceRequest`. Refactor `Invoice` to support optional `ServiceRequestId` FK.
- [x] **Add MediatR**: Add MediatR 12.x NuGet package to Application project and register in DI.
- [x] **Reject Command**: Create `RejectQuotationCommand` and Handler. Change `ServiceRequest` status to `Closed_Rejected`, auto-generate pending `Invoice` for "Inspection Fee" (fixed 150.00).
- [x] **Approve Command**: Create `ApproveQuotationCommand` and Handler. Change `ServiceRequest` status to `In_Progress`, mark Quotation as `Approved`, publish `QuotationApprovedEvent` via MediatR.
- [x] **Event Handler (Inventory/Purchasing)**: Create `AllocatePartsEventHandler` implementing `INotificationHandler<QuotationApprovedEvent>`. Loop through Part items, deduct stock if available, create `PurchaseNeed` for out-of-stock parts.
- [x] **EF Core + Migration**: Add `PurchaseNeed` DbSet, update `QuotationConfiguration` (Status), update `InvoiceConfiguration` (nullable FK), add `PurchaseNeedConfiguration`. Run migration `Phase4_ApprovalRejection`.
- [x] **API Controller**: Add `POST /api/quotations/{id}/approve` and `POST /api/quotations/{id}/reject` in a new `QuotationsController`.

## ⚙️ Phase 5: Infrastructure Wiring & MySQL Integration

- [x] **Database Setup**: Install `Pomelo.EntityFrameworkCore.MySql` in the Infrastructure layer. Update `appsettings.json` and `appsettings.Development.json` with a standard MySQL connection string (e.g., `Server=localhost;Database=WorkshopDb;User=root;Password=;`).
- [x] **Infrastructure DI**: Refactored `DependencyInjection.cs` in Infrastructure → renamed to `AddInfrastructureServices`. Registers `DbContext` (Pomelo MySQL, pinned 8.0.36), `AuditableEntityInterceptor` singleton, generic `IRepository<>` and `IUnitOfWork`.
- [x] **Application DI**: Refactored `DependencyInjection.cs` in Application → renamed to `AddApplicationServices`. Registers MediatR (auto-discover from assembly), FluentValidation validators (assembly scan via `AddValidatorsFromAssemblyContaining<>`), `IServiceRequestFactory`, all CQRS command handlers.
- [x] **API Wiring & Middleware**: Updated `Program.cs` — calls `AddApplicationServices()` and `AddInfrastructureServices()`. Global `IExceptionHandler` and `AddProblemDetails()` registered. `AddFluentValidationAutoValidation()` kept in API layer. Pipeline: `UseExceptionHandler() → UseHttpsRedirection() → UseAuthorization() → MapControllers()`.
- [x] **Migrations**: Removed SQL Server migration files, added `IDesignTimeDbContextFactory` for offline scaffolding. Ran `dotnet ef migrations add InitialCreate` ✅. Run `dotnet ef database update` after updating credentials in appsettings.
