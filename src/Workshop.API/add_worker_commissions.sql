USE WorkshopManagementDb;
ALTER TABLE ServiceRequests ADD ClosedAt datetime(6) NULL;

CREATE TABLE WorkerCommissions (
    Id char(36) COLLATE ascii_general_ci NOT NULL,
    WorkerId char(36) COLLATE ascii_general_ci NOT NULL,
    ServiceRequestId char(36) COLLATE ascii_general_ci NOT NULL,
    Amount decimal(65,30) NOT NULL,
    CreatedAt datetime(6) NOT NULL,
    UpdatedAt datetime(6) NULL,
    CONSTRAINT PK_WorkerCommissions PRIMARY KEY (Id),
    CONSTRAINT FK_WorkerCommissions_Mechanics_WorkerId FOREIGN KEY (WorkerId) REFERENCES Mechanics (Id) ON DELETE CASCADE,
    CONSTRAINT FK_WorkerCommissions_ServiceRequests_ServiceRequestId FOREIGN KEY (ServiceRequestId) REFERENCES ServiceRequests (Id) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

CREATE INDEX IX_WorkerCommissions_ServiceRequestId ON WorkerCommissions (ServiceRequestId);
CREATE INDEX IX_WorkerCommissions_WorkerId ON WorkerCommissions (WorkerId);

INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('20260404141813_AddWorkerCommissions', '8.0.25');
