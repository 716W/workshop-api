[⬅️ Back to Main README](../../README.md)

# 🚗 Workshop Business Flow & Vehicle Lifecycle

This document describes the end-to-end (7-scenario) lifecycle of a vehicle passing through our workshop system.

## 1. 📝 Reception & Check-In

- **Action**: Customer brings the vehicle to the workshop.
- **Process**: Receptionist captures customer details, vehicle info, and the primary complaint.
- **System Entity**: A **Service Request** is created (Status: `Registered` or `Open`).

## 2. 🔍 Inspection & Quoting

- **Action**: A mechanic or service advisor inspects the vehicle.
- **Process**: The necessary parts and labor are identified to resolve the issue. Prices, quantities, and descriptions are compiled.
- **System Entity**: A **Quotation** is generated and linked to the Service Request (Status: `Pending_Customer_Approval`).

## 3. ✅ Customer Approval

- **Action**: The quote is presented to the customer.
- **Process**: Customer approves (or rejects) the proposed repairs. If rejected, a basic inspection fee invoice may be generated.
- **System Entity**: Quotation becomes `Approved`. Service Request status changes to `In_Progress`.

## 4. 📦 Inventory Allocation & Procurements

- **Action**: Securing necessary parts for the approved repair.
- **Process**: System checks inventory. Available parts are allocated/deducted. Out-of-stock items automatically generate **Purchase Needs**.
- **System Entity**: Update **Inventory**, trigger **PurchaseNeed** records.

## 5. 🛠️ Repair Execution (Job Card)

- **Action**: The physical work begins.
- **Process**: Assigned mechanic(s) execute the tasks, logging time and confirming part usage.
- **System Entity**: A **Job Card** is active and updated.

## 6. 🧪 Quality Control (QC) & Testing

- **Action**: Post-repair validation.
- **Process**: A senior mechanic or foreman takes the vehicle for a test drive or performs a final checklist to assure quality standards.
- **System Entity**: Job Card status moved to `QC_Testing`, then finalized as `Completed`.

## 7. 💵 Invoicing, Payment & Release

- **Action**: Finalizing the service and releasing the vehicle.
- **Process**: An invoice is generated based on the approved quotation (and any add-ons). The customer pays, signs off, and collects their vehicle.
- **System Entity**: **Invoice** generated. Service Request status marked as `Closed_Completed`.
