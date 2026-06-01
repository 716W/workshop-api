[⬅️ Back to Main README](../../README.md)

# 🚀 Roadmap & Future Ideas

This document captures potential enhancements and feature expansions for the Workshop Management System. Items here are **not committed** — they represent engineering observations and business opportunities identified during development.

---

## Short-Term Improvements

### 🔧 Complete Remaining Module CRUD

Phases 14 and 15 have partial implementations. Completing write operations for Inventory and Customers would close the gap:

- `CreatePartCommand`, `UpdatePartCommand`
- `ResolvePurchaseNeedCommand`
- `UpdateCustomerCommand`, `UpdateVehicleCommand`
- `InventoryController` and `CustomersController` with full CRUD endpoints

### 📊 Dashboard Query

A single `GetDashboardSummaryQuery` returning:
- Total open requests by status
- Revenue this month (sum of paid invoices)
- Top worker by commissions earned
- Parts with stock below a threshold

This would power a management overview screen without any new domain logic.

### 🧪 Automated Tests

The codebase has been manually tested via `.http` files. Adding:
- Unit tests for all command handlers (using `xUnit` + `Moq`)
- Integration tests for the API layer (using `WebApplicationFactory<Program>`)

...would raise confidence and enable CI/CD pipelines.

---

## Medium-Term Features

### ☁️ Cloud File Storage

Replace `LocalFileStorageService` with an `AzureBlobStorageService` or `S3FileStorageService` — the `IFileStorageService` abstraction is already in place. No application or domain code changes required.

### 📧 Notification System

When a quotation is approved or a vehicle is ready for release, notify the customer via:
- Email (using SendGrid or AWS SES)
- SMS (using Twilio)

Implement as additional `INotificationHandler<QuotationApprovedEvent>` subscribers — zero changes to existing handlers.

### 🗓️ Appointments Module

Allow customers to book service slots in advance. Entities: `Appointment` (CustomerId, VehicleId, ScheduledAt, ServiceType, Status). Integrates with the `ServiceRequest` creation flow.

### 📱 Mobile-Friendly Response Shape

Add a separate mobile API profile (or API versioning) that returns leaner payloads for a potential Flutter or React Native app for mechanics.

---

## Long-Term Ideas

### 🏢 Multi-Tenant Architecture

Support multiple workshop branches under a single deployment. Adds a `TenantId` to all entities, with row-level filtering via EF Core global query filters.

### 🤖 AI-Assisted Quotation

Integrate with an LLM API to suggest repair items and estimated costs based on the vehicle make/model and described complaint. The suggestion would be editable before submission.

### 📦 Supplier Integration

Direct API integration with parts suppliers to:
- Check real-time stock and pricing
- Automatically raise purchase orders from `PurchaseNeed` records

### 📈 Analytics & Reporting

A dedicated read-optimized reporting database (CQRS read side with a separate store) for:
- Month-over-month revenue trends
- Vehicle return rates (repeat repairs)
- Mechanic performance metrics

---

*Ideas without business value justification should not be implemented. This list is a starting point for conversations, not a commitment.*
