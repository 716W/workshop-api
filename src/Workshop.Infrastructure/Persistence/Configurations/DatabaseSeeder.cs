using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Workshop.Domain.Entities;
using Workshop.Domain.Enums;

namespace Workshop.Infrastructure.Persistence.Configurations;

public static class DatabaseSeeder
{
    public static void SeedData(this ModelBuilder modelBuilder)
    {
        var now = new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

        var customerId = Guid.Parse("f39e31d3-356f-40e1-a070-9856f68c1870");
        var vehicleId = Guid.Parse("b15e21d3-356f-40e1-a070-9856f68c1871");
        var mechanicId = Guid.Parse("a55e21d3-356f-40e1-a070-9856f68c1872");
        var partId = Guid.Parse("c65e21d3-356f-40e1-a070-9856f68c1873");
        var repairRequestId = Guid.Parse("d75e21d3-356f-40e1-a070-9856f68c1874");
        var jobCardId = Guid.Parse("e85e21d3-356f-40e1-a070-9856f68c1875");
        var quotationId = Guid.Parse("f95e21d3-356f-40e1-a070-9856f68c1876");
        var quotationItemId = Guid.Parse("0a6e21d3-356f-40e1-a070-9856f68c1877");
        var invoiceId = Guid.Parse("1b6e21d3-356f-40e1-a070-9856f68c1878");
        var paymentId = Guid.Parse("2c6e21d3-356f-40e1-a070-9856f68c1879");
        var workerCommissionId = Guid.Parse("3d6e21d3-356f-40e1-a070-9856f68c187a");
        var purchaseNeedId = Guid.Parse("4e6e21d3-356f-40e1-a070-9856f68c187b");
        var historyId = Guid.Parse("5f6e21d3-356f-40e1-a070-9856f68c187c");
        var jobCardPartId = Guid.Parse("6a6e21d3-356f-40e1-a070-9856f68c187d");

        modelBuilder.Entity<Customer>().HasData(new
        {
            Id = customerId,
            FirstName = "John",
            LastName = "Doe",
            PhoneNumber = "555-0100",
            Email = "john.doe@example.com",
            CreatedAt = now
        });

        modelBuilder.Entity<Vehicle>().HasData(new
        {
            Id = vehicleId,
            CustomerId = customerId,
            Make = "Toyota",
            Model = "Camry",
            Year = 2022,
            LicensePlate = "XYZ-9876",
            PlateNumber = "XYZ-9876",
            VIN = "1HGCM82633A",
            VinNumber = "1HGCM82633A",
            CreatedAt = now
        });

        modelBuilder.Entity<Mechanic>().HasData(new
        {
            Id = mechanicId,
            FirstName = "Mike",
            LastName = "Smith",
            PhoneNumber = "555-0200",
            Specialty = "General Diagnostics",
            Specialization = "Engine Repair",
            EmployeeNumber = "EMP-1001",
            HireDate = now.AddYears(-2),
            IsAvailable = true,
            BaseSalary = 4500m,
            CommissionRate = 15m,
            CreatedAt = now
        });

        modelBuilder.Entity<Part>().HasData(new
        {
            Id = partId,
            Code = "BRK-PAD-001",
            PartNumber = "PN-12345",
            Name = "Premium Brake Pads",
            Description = "Front ceramic brake pads set",
            UnitCost = 25m,
            UnitPrice = 55m,
            RetailPrice = 55m,
            QuantityInStock = 120,
            ReorderThreshold = 20,
            ReorderLevel = 20,
            Location = "Shelf A2",
            CreatedAt = now
        });

        modelBuilder.Entity<RepairRequest>().HasData(new
        {
            Id = repairRequestId,
            VehicleId = vehicleId,
            RepairDescription = "Squeaking noise from front wheels",
            InspectionNotes = "Needs new brake pads",
            CustomerId = customerId,
            MechanicId = mechanicId,
            Price = 120m,
            CommissionType = CommissionType.Percentage,
            CommissionValue = 15m,
            RequestType = RequestType.Repair,
            Status = ServiceRequestStatus.Closed_Success,
            Date = now,
            CreatedAt = now
        });

        modelBuilder.Entity<ServiceRequestStatusHistory>().HasData(new
        {
            Id = historyId,
            ServiceRequestId = repairRequestId,
            OldStatus = ServiceRequestStatus.Open,
            NewStatus = ServiceRequestStatus.Closed_Success,
            Notes = "Completed successfully",
            CreatedAt = now
        });

        modelBuilder.Entity<Quotation>().HasData(new
        {
            Id = quotationId,
            ServiceRequestId = repairRequestId,
            Notes = "Estimate for front brake pads replacement",
            Status = QuotationStatus.Approved,
            GrandTotal = 55m,
            CreatedAt = now
        });

        modelBuilder.Entity<QuotationItem>().HasData(new
        {
            Id = quotationItemId,
            QuotationId = quotationId,
            Type = QuotationItemType.Part,
            Description = "Premium Brake Pads",
            Quantity = 1,
            UnitPrice = 55m,
            TotalPrice = 55m,
            CreatedAt = now
        });

        modelBuilder.Entity<PurchaseNeed>().HasData(new
        {
            Id = purchaseNeedId,
            PartName = "Premium Brake Pads",
            Quantity = 10,
            ServiceRequestId = repairRequestId,
            JobCardId = jobCardId,
            DateRequested = now,
            CreatedAt = now
        });

        modelBuilder.Entity<JobCard>().HasData(new
        {
            Id = jobCardId,
            JobNumber = "JC-2026-0001",
            Description = "Replace front brake pads",
            Status = JobCardStatus.Released,
            CheckedInAt = now,
            EstimatedCost = 55m,
            FinalCost = 55m,
            VehicleId = vehicleId,
            MechanicId = mechanicId,
            CreatedAt = now
        });

        modelBuilder.Entity<JobCardPart>().HasData(new
        {
            Id = jobCardPartId,
            JobCardId = jobCardId,
            PartId = partId,
            Quantity = 1,
            UnitPriceAtTime = 55m,
            CreatedAt = now
        });

        modelBuilder.Entity<Invoice>().HasData(new
        {
            Id = invoiceId,
            InvoiceNumber = "INV-2026-0001",
            SubTotal = 55m,
            TaxAmount = 8.25m,
            Discount = 0m,
            TotalAmount = 63.25m,
            Status = InvoiceStatus.Paid,
            IssuedAt = now,
            ServiceRequestId = repairRequestId,
            JobCardId = jobCardId,
            CreatedAt = now
        });

        modelBuilder.Entity<Payment>().HasData(new
        {
            Id = paymentId,
            InvoiceId = invoiceId,
            Amount = 63.25m,
            PaymentMethod = PaymentMethod.Card,
            PaymentDate = now,
            TransactionReference = "TXN-9988776655",
            CreatedAt = now
        });

        modelBuilder.Entity<WorkerCommission>().HasData(new
        {
            Id = workerCommissionId,
            WorkerId = mechanicId,
            ServiceRequestId = repairRequestId,
            Amount = 18m,
            CreatedAt = now
        });

        // Seed Business Roles
        var roles = new[] { "Manager", "Receptionist", "Mechanic", "QC_Inspector", "Inventory_Manager", "Accountant" };
        var roleIdStart = 1;
        foreach (var role in roles)
        {
            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = roleIdStart.ToString(),
                Name = role,
                NormalizedName = role.ToUpper(),
                ConcurrencyStamp = Guid.NewGuid().ToString()
            });
            roleIdStart++;
        }
    }
}
