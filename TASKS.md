# Workshop System Development Plan

Current Status: Phase 2 (Scenario 1) COMPLETE

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
