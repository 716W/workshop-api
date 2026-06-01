-- Phase 18 migrations — targeted SQL (MySQL 8.0)
-- Adds CreatedBy/UpdatedBy to tables that don't have them yet,
-- creates Attachments table, and records migrations in history.

ALTER TABLE `ServiceRequests`  ADD COLUMN `CreatedBy` longtext CHARACTER SET utf8mb4 NULL, ADD COLUMN `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL;
ALTER TABLE `Quotations`       ADD COLUMN `CreatedBy` longtext CHARACTER SET utf8mb4 NULL, ADD COLUMN `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL;
ALTER TABLE `QuotationItems`   ADD COLUMN `CreatedBy` longtext CHARACTER SET utf8mb4 NULL, ADD COLUMN `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL;
ALTER TABLE `Parts`            ADD COLUMN `CreatedBy` longtext CHARACTER SET utf8mb4 NULL, ADD COLUMN `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL;
ALTER TABLE `Mechanics`        ADD COLUMN `CreatedBy` longtext CHARACTER SET utf8mb4 NULL, ADD COLUMN `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL;
ALTER TABLE `JobCards`         ADD COLUMN `CreatedBy` longtext CHARACTER SET utf8mb4 NULL, ADD COLUMN `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL;
ALTER TABLE `JobCardParts`     ADD COLUMN `CreatedBy` longtext CHARACTER SET utf8mb4 NULL, ADD COLUMN `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL;
ALTER TABLE `Invoices`         ADD COLUMN `CreatedBy` longtext CHARACTER SET utf8mb4 NULL, ADD COLUMN `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL;
ALTER TABLE `Customers`        ADD COLUMN `CreatedBy` longtext CHARACTER SET utf8mb4 NULL, ADD COLUMN `UpdatedBy` longtext CHARACTER SET utf8mb4 NULL;

CREATE TABLE IF NOT EXISTS `Attachments` (
    `Id`               char(36) COLLATE ascii_general_ci NOT NULL,
    `ServiceRequestId` char(36) COLLATE ascii_general_ci NOT NULL,
    `FileName`         varchar(255)  CHARACTER SET utf8mb4 NOT NULL,
    `FilePath`         varchar(1000) CHARACTER SET utf8mb4 NOT NULL,
    `ContentType`      varchar(100)  CHARACTER SET utf8mb4 NOT NULL,
    `UploadedAt`       datetime(6) NOT NULL,
    `UploadedBy`       varchar(450)  CHARACTER SET utf8mb4 NULL,
    `CreatedAt`        datetime(6) NOT NULL,
    `UpdatedAt`        datetime(6) NULL,
    `CreatedBy`        varchar(450)  CHARACTER SET utf8mb4 NULL,
    `UpdatedBy`        varchar(450)  CHARACTER SET utf8mb4 NULL,
    CONSTRAINT `PK_Attachments` PRIMARY KEY (`Id`),
    CONSTRAINT `FK_Attachments_ServiceRequests`
        FOREIGN KEY (`ServiceRequestId`) REFERENCES `ServiceRequests` (`Id`) ON DELETE CASCADE
) CHARACTER SET=utf8mb4;

ALTER TABLE `Attachments` ADD INDEX `IX_Attachments_ServiceRequestId` (`ServiceRequestId`);

INSERT IGNORE INTO `__EFMigrationsHistory` (MigrationId, ProductVersion)
VALUES
    ('20260531161140_AddAuditUserFields', '8.0.36'),
    ('20260531161218_AddAttachments',     '8.0.36');

SELECT 'Phase 18 migrations applied successfully' AS Result;
