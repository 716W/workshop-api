using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Workshop.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class seedingData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RepairDescription",
                table: "ServiceRequests",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "PurchaseDescription",
                table: "ServiceRequests",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "InspectionNotes",
                table: "ServiceRequests",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(2000)",
                oldMaxLength: 2000)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "CreatedAt", "Email", "FirstName", "LastName", "PhoneNumber", "UpdatedAt" },
                values: new object[] { new Guid("f39e31d3-356f-40e1-a070-9856f68c1870"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "john.doe@example.com", "John", "Doe", "555-0100", null });

            migrationBuilder.InsertData(
                table: "Mechanics",
                columns: new[] { "Id", "CreatedAt", "FirstName", "IsAvailable", "LastName", "PhoneNumber", "Specialization", "UpdatedAt" },
                values: new object[] { new Guid("a55e21d3-356f-40e1-a070-9856f68c1872"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Mike", true, "Smith", "555-0200", "Engine Repair", null });

            migrationBuilder.InsertData(
                table: "Parts",
                columns: new[] { "Id", "CreatedAt", "Description", "Name", "PartNumber", "QuantityInStock", "ReorderLevel", "UnitPrice", "UpdatedAt" },
                values: new object[] { new Guid("c65e21d3-356f-40e1-a070-9856f68c1873"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Front ceramic brake pads set", "Premium Brake Pads", "PN-12345", 120, 20, 55m, null });

            migrationBuilder.InsertData(
                table: "Vehicles",
                columns: new[] { "Id", "CreatedAt", "CustomerId", "Make", "Model", "PlateNumber", "UpdatedAt", "VinNumber", "Year" },
                values: new object[] { new Guid("b15e21d3-356f-40e1-a070-9856f68c1871"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("f39e31d3-356f-40e1-a070-9856f68c1870"), "Toyota", "Camry", "XYZ-9876", null, "1HGCM82633A", 2022 });

            migrationBuilder.InsertData(
                table: "JobCards",
                columns: new[] { "Id", "CheckedInAt", "CompletedAt", "CreatedAt", "Description", "EstimatedCost", "FinalCost", "InspectionNotes", "JobNumber", "MechanicId", "Status", "UpdatedAt", "VehicleId" },
                values: new object[] { new Guid("e85e21d3-356f-40e1-a070-9856f68c1875"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Replace front brake pads", 55m, 55m, null, "JC-2026-0001", new Guid("a55e21d3-356f-40e1-a070-9856f68c1872"), "Released", null, new Guid("b15e21d3-356f-40e1-a070-9856f68c1871") });

            migrationBuilder.InsertData(
                table: "ServiceRequests",
                columns: new[] { "Id", "ClosedAt", "CommissionType", "CommissionValue", "CreatedAt", "CustomerId", "Date", "MechanicId", "Price", "RepairDescription", "RequestType", "Status", "UpdatedAt", "VehicleId" },
                values: new object[] { new Guid("d75e21d3-356f-40e1-a070-9856f68c1874"), null, "Percentage", 15m, new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("f39e31d3-356f-40e1-a070-9856f68c1870"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("a55e21d3-356f-40e1-a070-9856f68c1872"), 120m, "Squeaking noise from front wheels", "Repair", "Closed_Success", null, new Guid("b15e21d3-356f-40e1-a070-9856f68c1871") });

            migrationBuilder.InsertData(
                table: "Invoices",
                columns: new[] { "Id", "CreatedAt", "Discount", "InvoiceNumber", "IssuedAt", "JobCardId", "PaidAt", "ServiceRequestId", "Status", "SubTotal", "TaxAmount", "TotalAmount", "UpdatedAt" },
                values: new object[] { new Guid("1b6e21d3-356f-40e1-a070-9856f68c1878"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), 0m, "INV-2026-0001", new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("e85e21d3-356f-40e1-a070-9856f68c1875"), null, new Guid("d75e21d3-356f-40e1-a070-9856f68c1874"), "Paid", 55m, 8.25m, 63.25m, null });

            migrationBuilder.InsertData(
                table: "JobCardParts",
                columns: new[] { "Id", "CreatedAt", "JobCardId", "PartId", "Quantity", "UnitPriceAtTime", "UpdatedAt" },
                values: new object[] { new Guid("6a6e21d3-356f-40e1-a070-9856f68c187d"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("e85e21d3-356f-40e1-a070-9856f68c1875"), new Guid("c65e21d3-356f-40e1-a070-9856f68c1873"), 1, 55m, null });

            migrationBuilder.InsertData(
                table: "PurchaseNeeds",
                columns: new[] { "Id", "CreatedAt", "DateRequested", "JobCardId", "PartName", "Quantity", "ServiceRequestId", "UpdatedAt" },
                values: new object[] { new Guid("4e6e21d3-356f-40e1-a070-9856f68c187b"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("e85e21d3-356f-40e1-a070-9856f68c1875"), "Premium Brake Pads", 10, new Guid("d75e21d3-356f-40e1-a070-9856f68c1874"), null });

            migrationBuilder.InsertData(
                table: "Quotations",
                columns: new[] { "Id", "CreatedAt", "GrandTotal", "Notes", "ServiceRequestId", "Status", "UpdatedAt" },
                values: new object[] { new Guid("f95e21d3-356f-40e1-a070-9856f68c1876"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), 55m, "Estimate for front brake pads replacement", new Guid("d75e21d3-356f-40e1-a070-9856f68c1874"), 1, null });

            migrationBuilder.InsertData(
                table: "ServiceRequestStatusHistories",
                columns: new[] { "Id", "CreatedAt", "NewStatus", "Notes", "OldStatus", "ServiceRequestId", "UpdatedAt" },
                values: new object[] { new Guid("5f6e21d3-356f-40e1-a070-9856f68c187c"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Closed_Success", "Completed successfully", "Open", new Guid("d75e21d3-356f-40e1-a070-9856f68c1874"), null });

            migrationBuilder.InsertData(
                table: "WorkerCommissions",
                columns: new[] { "Id", "Amount", "CreatedAt", "ServiceRequestId", "UpdatedAt", "WorkerId" },
                values: new object[] { new Guid("3d6e21d3-356f-40e1-a070-9856f68c187a"), 18m, new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("d75e21d3-356f-40e1-a070-9856f68c1874"), null, new Guid("a55e21d3-356f-40e1-a070-9856f68c1872") });

            migrationBuilder.InsertData(
                table: "Payments",
                columns: new[] { "Id", "Amount", "CreatedAt", "InvoiceId", "PaymentDate", "PaymentMethod", "TransactionReference", "UpdatedAt" },
                values: new object[] { new Guid("2c6e21d3-356f-40e1-a070-9856f68c1879"), 63.25m, new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), new Guid("1b6e21d3-356f-40e1-a070-9856f68c1878"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Card", "TXN-9988776655", null });

            migrationBuilder.InsertData(
                table: "QuotationItems",
                columns: new[] { "Id", "CreatedAt", "Description", "Quantity", "QuotationId", "TotalPrice", "Type", "UnitPrice", "UpdatedAt" },
                values: new object[] { new Guid("0a6e21d3-356f-40e1-a070-9856f68c1877"), new DateTime(2026, 1, 1, 12, 0, 0, 0, DateTimeKind.Utc), "Premium Brake Pads", 1, new Guid("f95e21d3-356f-40e1-a070-9856f68c1876"), 55m, "Part", 55m, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "JobCardParts",
                keyColumn: "Id",
                keyValue: new Guid("6a6e21d3-356f-40e1-a070-9856f68c187d"));

            migrationBuilder.DeleteData(
                table: "Payments",
                keyColumn: "Id",
                keyValue: new Guid("2c6e21d3-356f-40e1-a070-9856f68c1879"));

            migrationBuilder.DeleteData(
                table: "PurchaseNeeds",
                keyColumn: "Id",
                keyValue: new Guid("4e6e21d3-356f-40e1-a070-9856f68c187b"));

            migrationBuilder.DeleteData(
                table: "QuotationItems",
                keyColumn: "Id",
                keyValue: new Guid("0a6e21d3-356f-40e1-a070-9856f68c1877"));

            migrationBuilder.DeleteData(
                table: "ServiceRequestStatusHistories",
                keyColumn: "Id",
                keyValue: new Guid("5f6e21d3-356f-40e1-a070-9856f68c187c"));

            migrationBuilder.DeleteData(
                table: "WorkerCommissions",
                keyColumn: "Id",
                keyValue: new Guid("3d6e21d3-356f-40e1-a070-9856f68c187a"));

            migrationBuilder.DeleteData(
                table: "Invoices",
                keyColumn: "Id",
                keyValue: new Guid("1b6e21d3-356f-40e1-a070-9856f68c1878"));

            migrationBuilder.DeleteData(
                table: "Parts",
                keyColumn: "Id",
                keyValue: new Guid("c65e21d3-356f-40e1-a070-9856f68c1873"));

            migrationBuilder.DeleteData(
                table: "Quotations",
                keyColumn: "Id",
                keyValue: new Guid("f95e21d3-356f-40e1-a070-9856f68c1876"));

            migrationBuilder.DeleteData(
                table: "JobCards",
                keyColumn: "Id",
                keyValue: new Guid("e85e21d3-356f-40e1-a070-9856f68c1875"));

            migrationBuilder.DeleteData(
                table: "ServiceRequests",
                keyColumn: "Id",
                keyValue: new Guid("d75e21d3-356f-40e1-a070-9856f68c1874"));

            migrationBuilder.DeleteData(
                table: "Mechanics",
                keyColumn: "Id",
                keyValue: new Guid("a55e21d3-356f-40e1-a070-9856f68c1872"));

            migrationBuilder.DeleteData(
                table: "Vehicles",
                keyColumn: "Id",
                keyValue: new Guid("b15e21d3-356f-40e1-a070-9856f68c1871"));

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: new Guid("f39e31d3-356f-40e1-a070-9856f68c1870"));

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "RepairDescription",
                keyValue: null,
                column: "RepairDescription",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "RepairDescription",
                table: "ServiceRequests",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "PurchaseDescription",
                keyValue: null,
                column: "PurchaseDescription",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "PurchaseDescription",
                table: "ServiceRequests",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "ServiceRequests",
                keyColumn: "InspectionNotes",
                keyValue: null,
                column: "InspectionNotes",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "InspectionNotes",
                table: "ServiceRequests",
                type: "varchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
