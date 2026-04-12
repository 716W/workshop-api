# Workshop System Development Plan

Current Status: Phase 12 (API Standardization) COMPLETE ✅

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

## 🔄 Phase 5: Scenario 3 - Approval, Rejection & Inventory

- [x] **Domain Updates**: Add `Status` (Pending, Approved, Rejected) to `Quotation`. Create a `PurchaseNeed` entity (Id, PartId/Name, Quantity, ServiceRequestId, DateRequested, IsResolved) for tracking out-of-stock items. Create a Domain Event `QuotationApprovedEvent` (inherits `INotification` from MediatR) containing the QuotationId.
- [x] **Reject Command**: Create `RejectQuotationCommand` and Handler. It changes the `ServiceRequest` status to `Closed_Rejected` and can optionally log an "Inspection Fee".
- [x] **Approve Command**: Create `ApproveQuotationCommand` and Handler. It changes `ServiceRequest` status to `In_Progress`, marks the `Quotation` as `Approved`, and publishes `QuotationApprovedEvent` via MediatR.
- [x] **Inventory Event Handler**: Create `AllocatePartsEventHandler` (`INotificationHandler<QuotationApprovedEvent>`). When triggered, it loops through the `Part` items in the quotation. (For now, assume parts are out-of-stock to test the logic) -> It creates a `PurchaseNeed` record using the repository so the purchasing department knows to buy them.
- [x] **API Controller**: Add `POST /api/quotations/{id}/approve` and `POST /api/quotations/{id}/reject` in the appropriate controller.
- [x] **Testing**: Update `tests/Manual/WorkshopScenarios.http` to test the Approve and Reject endpoints.

## 🔧 Phase 6: Scenario 4 - Repair Execution & Status Tracking

- [x] **Database Setup**: Install `Pomelo.EntityFrameworkCore.MySql` in the Infrastructure layer. Update `appsettings.json` and `appsettings.Development.json` with a standard MySQL connection string (e.g., `Server=localhost;Database=WorkshopDb;User=root;Password=;`).
- [x] **Infrastructure DI**: Refactored `DependencyInjection.cs` in Infrastructure → renamed to `AddInfrastructureServices`. Registers `DbContext` (Pomelo MySQL, pinned 8.0.36), `AuditableEntityInterceptor` singleton, generic `IRepository<>` and `IUnitOfWork`.
- [x] **Application DI**: Refactored `DependencyInjection.cs` in Application → renamed to `AddApplicationServices`. Registers MediatR (auto-discover from assembly), FluentValidation validators (assembly scan via `AddValidatorsFromAssemblyContaining<>`), `IServiceRequestFactory`, all CQRS command handlers.
- [x] **API Wiring & Middleware**: Updated `Program.cs` — calls `AddApplicationServices()` and `AddInfrastructureServices()`. Global `IExceptionHandler` and `AddProblemDetails()` registered. `AddFluentValidationAutoValidation()` kept in API layer. Pipeline: `UseExceptionHandler() → UseHttpsRedirection() → UseAuthorization() → MapControllers()`.
- [x] **Migrations**: Removed SQL Server migration files, added `IDesignTimeDbContextFactory` for offline scaffolding. Ran `dotnet ef migrations add InitialCreate` ✅. Run `dotnet ef database update` after updating credentials in appsettings.

- [x] **Domain Entities**: Create a `ServiceRequestStatusHistory` entity (Id, ServiceRequestId, OldStatus, NewStatus, Notes, CreatedAt). Ensure the relationship is configured in the `DbContext`.
- [x] **Domain Enums**: Expand the `Status` Enum (if not already done) to include: `Repairing`, `Waiting_For_Parts`, `External_Work`, and `Ready_For_QC`.
- [x] **CQRS & DTOs**: Create `UpdateServiceRequestStatusDto` (NewStatus, Notes). Create `UpdateServiceRequestStatusCommand` and its Handler.
- [x] **Business Rules (Handler)**: The handler must fetch the request, generate a new `ServiceRequestStatusHistory` record, update the main request's status to the new one, and save both via the repository.
- [x] **API Controller**: Add a `PATCH /api/requests/{id}/status` endpoint in the `OperationsController` to handle this command.
- [x] **Migrations**: Run a new EF Core migration (`AddStatusHistory`) to create the new table in MySQL.
- [x] **Testing**: Update `tests/Manual/WorkshopScenarios.http` with a scenario showing a status update (e.g., to `Waiting_For_Parts` with a note).

## 🔍 Phase 7: Scenario 5 - Quality Control (QC)
- [x] **Domain Enums**: Add `Ready_For_Invoicing` to the `Status` Enum (if not present).
- [x] **CQRS & DTOs**: Create `PerformQCDto` (bool IsPassed, string Notes). Create `PerformQCCommand` and its Handler.
- [x] **Business Rules (Handler)**: The handler checks if the current status is `Ready_For_QC`.
  - If `IsPassed` == true, set new status to `Ready_For_Invoicing`.
  - If `IsPassed` == false, set new status to `Repairing` (or `QC_Failed`).
  - Call the `UpdateStatus` method (from Phase 6) to ensure the status change and notes are logged in `ServiceRequestStatusHistory`.
- [x] **API Controller**: Add `POST /api/requests/{id}/qc` in the `OperationsController`.
- [x] **Testing**: Update `tests/Manual/WorkshopScenarios.http` with a QC scenario (e.g., failing it first with a note, then passing it).

## 💳 Phase 8: Scenario 6 - Invoicing & Payment
- [x] **Domain Entities**: Create `Invoice` (Id, ServiceRequestId, SubTotal, TaxAmount, Discount, TotalAmount, Status [Unpaid, Paid]). Create `Payment` (Id, InvoiceId, Amount, PaymentMethod [Cash, Card, Transfer], PaymentDate).
- [x] **Generate Invoice Command**: Create `GenerateInvoiceCommand` and Handler. It fetches the Approved `Quotation` for the request, calculates SubTotal, adds 15% Tax, creates the `Invoice`, and changes `ServiceRequest` status to `Pending_Payment`.
- [x] **Pay Invoice Command**: Create `ProcessPaymentCommand` (InvoiceId, Amount, PaymentMethod) and Handler. It creates a `Payment` record. If total payments >= `Invoice.TotalAmount`, mark `Invoice` as `Paid` and update `ServiceRequest` status to `Ready_For_Release`.
- [x] **API Controller**: Add `POST /api/requests/{id}/invoice` and `POST /api/invoices/{id}/pay` in a new `BillingController`.
- [x] **Migrations**: Run a new EF Core migration (`AddInvoicing`) to create the Invoice and Payment tables.
- [x] **Testing**: Update `tests/Manual/WorkshopScenarios.http` to generate an invoice and pay it in full.

## 🏁 Phase 9: Scenario 7 - Vehicle Release & Closure
- [x] **Domain Updates**: Add `Closed_Success` to the `Status` Enum. Add `ClosedAt` (nullable DateTime) to `ServiceRequest`. Create a `WorkerCommission` entity (Id, WorkerId, ServiceRequestId, Amount, CreatedAt).
- [x] **CQRS Command**: Create `CloseServiceRequestCommand` and Handler.
- [x] **Business Rules (Handler)**: Ensure the request status is `Ready_For_Release`. Change status to `Closed_Success` and set `ClosedAt` to `DateTime.UtcNow`. Calculate the mechanic's commission (e.g., if `Fixed`, use `CommissionValue`; if `Percentage`, calculate based on `Invoice.SubTotal` or Labor total) and create a `WorkerCommission` record.
- [x] **API Controller**: Add `POST /api/requests/{id}/release` in the `ReceptionController` (or OperationsController).
- [x] **Migrations**: Run a final EF Core migration (`AddWorkerCommissions`) to update the database.
- [x] **Testing**: Add the final API call to `tests/Manual/WorkshopScenarios.http` to complete the full lifecycle!

## 🗂️ Phase 10: Refactoring & Comprehensive Testing
- [x] **Domain Organization**: Group classes in the `Domain` project into descriptive folders: `/Entities`, `/Enums`, `/Events`, and `/ValueObjects` (if any). Update namespaces accordingly.
- [x] **Application Organization (Feature Folders)**: Refactor the `Application` project using Feature folders. Create a `/Features` folder with subfolders like `ServiceRequests`, `Quotations`, `Invoicing`, and `QC`. Move the relevant Commands, Handlers, and DTOs into these feature folders. Update namespaces globally to ensure the solution compiles successfully.
- [x] **Infrastructure Organization**: Ensure `Infrastructure` is cleanly grouped into `/Persistence` (DbContext, Interceptors, Migrations), `/Repositories`, and `/Services` (if any).
- [x] **HTTP Testing (Validations)**: Update `tests/Manual/WorkshopScenarios.http` with "Sad Path" Validation cases (e.g., POST a Request with missing/invalid data, negative quantity in Quotation).
- [x] **HTTP Testing (Business Rules)**: Add "Sad Path" Business logic cases to the `.http` file (e.g., Try to approve a quotation for a closed request, try to pay an already paid invoice, try to release a vehicle that is not `Ready_For_Release`).
- [x] **Final Build Check**: Run `dotnet build` to guarantee no namespace or missing reference errors exist after the folder restructuring.

## 📚 Phase 11: Project Documentation (Docs-as-Code)
- [x] **Setup Docs Structure**: Create a `docs` folder at the root of the project. Inside it, create subfolders: `01-Business-Flows`, `02-Features`, `03-Architecture`, and `04-Future-Ideas`.
- [x] **Document Workshop Flow**: Create a file named `workshop-lifecycle.md` inside `docs/01-Business-Flows`. Write down the complete 7-scenario lifecycle of a vehicle in the workshop (from Check-In to Release, including QC and Invoicing) based on our previous discussions. Use clean Markdown with headers, bullet points, and emojis for readability.
- [x] **Main README**: Update the root `README.md` (or create an index in the `docs` folder) to link to this new `workshop-lifecycle.md` file.

## 🏗️ Phase 12: API Standardization (Routes & Responses)
- [x] **Unified Routing**: Created static class `ApiRoutes` in `src/Workshop.API/Routes/ApiRoutes.cs`. Defines constants for all existing routes via nested static classes per feature (`Reception`, `Operations`, `Quotations`, `Inventory`, `JobCards`). Each class exposes a `Base` constant for the `[Route]` attribute and named constants for each action template.
- [x] **Unified Response Contracts**: Created `ApiResponse<T>` and non-generic `ApiResponse` in `src/Workshop.API/Contracts/ApiResponse.cs` (fields: `Data`, `Message`, `IsSuccess`). Created `PagedResponse<T>` in `src/Workshop.API/Contracts/PagedResponse.cs` (adds `PageNumber`, `PageSize`, `TotalRecords`, computed `TotalPages`).
- [x] **Base API Controller**: Created `BaseApiController` in `src/Workshop.API/Controllers/BaseApiController.cs` inheriting `ControllerBase` with `[ApiController]`. Implements `HandleResult<T>(Result<T>)` → 200/400/404, `HandleCreated<T>(Result<T>, string)` → 201/400/404, and `HandlePagedResult<TItem>(Result<PagedResult<TItem>>)` → 200/400/404. Maps internal `Result<T>` to `ApiResponse<T>`-wrapped HTTP responses. The `GlobalExceptionHandler` (Phase 1.5) remains untouched — it handles uncaught exceptions and returns 500 ProblemDetails.
- [x] **Refactor Existing Controllers**: Updated `ReceptionController`, `OperationsController`, and `QuotationsController` to inherit `BaseApiController`. Replaced all hardcoded `[Route(...)]` strings with `ApiRoutes.*` constants. Action methods now call `HandleCreated(result, location)` or `HandleResult(result)` — zero manual `if (!result.IsSuccess)` branches remaining.
- [x] **Test File Update**: Verified `tests/Manual/WorkshopScenarios.http` is unchanged — all route URLs (`/api/reception/create`, `/api/requests/{id}/quotations`) are identical after standardization. No edit required. Build: ✅ 0 errors, 0 warnings.
