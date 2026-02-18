# 🛠 Workshop System Development Plan

Current Status: 🟡 IN PROGRESS (Phase 1: Foundation)

## 📌 Rules for AI Agent

1. Read this file BEFORE doing anything.
2. Only work on tasks marked as [ ] inside the "Current Phase".
3. Do NOT jump to future phases.
4. When a task is done, mark it [x] and update "Current Status".

## 🚀 Phase 1: Foundation (Dependencies: None)

- [x] **Setup Solution**: Create .NET 8 Web API with Clean Architecture (Domain, Application, Infrastructure, API).
- [ ] **Setup Git**: Initialize git and create .gitignore.
- [ ] **Setup Database**: Configure SQL Server connection string in appsettings.json.

## 🏗 Phase 2: Domain Entities (Dependencies: Phase 1)

- [ ] **Create Customer & Vehicle**: Define entities and relationship (1:N).
- [ ] **Create Mechanic & Part**: Define entities.
- [ ] **Create JobCard**: The core entity linking Vehicle, Mechanic, and Status.
- [ ] **EF Core Context**: Register all DbSets and configurations.

## ⚙️ Phase 3: Core Logic (Dependencies: Phase 2)

- [ ] **Repository Pattern**: Implement Generic Repository & Unit of Work.
- [ ] **JobCard Service**: Implement "Check-in" and "Inspection" logic.
- [ ] **Inventory Service**: Implement "Consume Part" logic.

## 🔌 Phase 4: API & Exposure (Dependencies: Phase 3)

- [ ] **Controllers**: Create Endpoints for JobCards.
- [ ] **DTOs & Validation**: Use FluentValidation.
