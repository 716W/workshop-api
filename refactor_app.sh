#!/bin/bash

APP_DIR="src/Workshop.Application"
FEAT_DIR="$APP_DIR/Features"

mkdir -p "$FEAT_DIR"/{ServiceRequests,Quotations,Invoicing,QC,Inventory}/{Commands,DTOs,Validators,Handlers,Interfaces,Services,Events}

# Move ServiceRequests
mv "$APP_DIR/Commands/CreateServiceRequestCommand.cs" "$FEAT_DIR/ServiceRequests/Commands/"
mv "$APP_DIR/Commands/CloseServiceRequestCommand.cs" "$FEAT_DIR/ServiceRequests/Commands/"
mv "$APP_DIR/Commands/UpdateServiceRequestStatusCommand.cs" "$FEAT_DIR/ServiceRequests/Commands/"
mv "$APP_DIR/DTOs/CreateServiceRequestDto.cs" "$FEAT_DIR/ServiceRequests/DTOs/"
mv "$APP_DIR/DTOs/UpdateServiceRequestStatusDto.cs" "$FEAT_DIR/ServiceRequests/DTOs/"
mv "$APP_DIR/Handlers/CreateServiceRequestCommandHandler.cs" "$FEAT_DIR/ServiceRequests/Handlers/"
mv "$APP_DIR/Handlers/CloseServiceRequestCommandHandler.cs" "$FEAT_DIR/ServiceRequests/Handlers/"
mv "$APP_DIR/Handlers/UpdateServiceRequestStatusCommandHandler.cs" "$FEAT_DIR/ServiceRequests/Handlers/"
mv "$APP_DIR/Validators/CreateServiceRequestValidator.cs" "$FEAT_DIR/ServiceRequests/Validators/"
mv "$APP_DIR/Validators/UpdateServiceRequestStatusValidator.cs" "$FEAT_DIR/ServiceRequests/Validators/"
mv "$APP_DIR/Interfaces/IJobCardService.cs" "$FEAT_DIR/ServiceRequests/Interfaces/"
mv "$APP_DIR/Interfaces/IServiceRequestFactory.cs" "$FEAT_DIR/ServiceRequests/Interfaces/"
mv "$APP_DIR/Services/JobCardService.cs" "$FEAT_DIR/ServiceRequests/Services/"
mv "$APP_DIR/Services/ServiceRequestFactory.cs" "$FEAT_DIR/ServiceRequests/Services/"

# Move Quotations
mv "$APP_DIR/Commands/GenerateQuotationCommand.cs" "$FEAT_DIR/Quotations/Commands/"
mv "$APP_DIR/Commands/ApproveQuotationCommand.cs" "$FEAT_DIR/Quotations/Commands/"
mv "$APP_DIR/Commands/RejectQuotationCommand.cs" "$FEAT_DIR/Quotations/Commands/"
mv "$APP_DIR/DTOs/CreateQuotationDto.cs" "$FEAT_DIR/Quotations/DTOs/"
mv "$APP_DIR/DTOs/QuotationItemDto.cs" "$FEAT_DIR/Quotations/DTOs/"
mv "$APP_DIR/Handlers/GenerateQuotationCommandHandler.cs" "$FEAT_DIR/Quotations/Handlers/"
mv "$APP_DIR/Handlers/ApproveQuotationCommandHandler.cs" "$FEAT_DIR/Quotations/Handlers/"
mv "$APP_DIR/Handlers/RejectQuotationCommandHandler.cs" "$FEAT_DIR/Quotations/Handlers/"
mv "$APP_DIR/Handlers/AllocatePartsEventHandler.cs" "$FEAT_DIR/Quotations/Events/"
mv "$APP_DIR/Validators/CreateQuotationValidator.cs" "$FEAT_DIR/Quotations/Validators/"

# Move Invoicing
mv "$APP_DIR/Commands/GenerateInvoiceCommand.cs" "$FEAT_DIR/Invoicing/Commands/"
mv "$APP_DIR/Commands/ProcessPaymentCommand.cs" "$FEAT_DIR/Invoicing/Commands/"
mv "$APP_DIR/Handlers/GenerateInvoiceCommandHandler.cs" "$FEAT_DIR/Invoicing/Handlers/"
mv "$APP_DIR/Handlers/ProcessPaymentCommandHandler.cs" "$FEAT_DIR/Invoicing/Handlers/"

# Move QC
mv "$APP_DIR/Commands/PerformQCCommand.cs" "$FEAT_DIR/QC/Commands/"
mv "$APP_DIR/DTOs/PerformQCDto.cs" "$FEAT_DIR/QC/DTOs/"
mv "$APP_DIR/Handlers/PerformQCCommandHandler.cs" "$FEAT_DIR/QC/Handlers/"

# Move Inventory
mv "$APP_DIR/Interfaces/IInventoryService.cs" "$FEAT_DIR/Inventory/Interfaces/"
mv "$APP_DIR/Services/InventoryService.cs" "$FEAT_DIR/Inventory/Services/"

# Clean up empty dirs
rmdir "$APP_DIR/Commands" "$APP_DIR/DTOs" "$APP_DIR/Handlers" "$APP_DIR/Validators" "$APP_DIR/Services" 2>/dev/null || true

# We will leave `Interfaces` as it has `ICommandHandler.cs`.
echo "Moved files."
