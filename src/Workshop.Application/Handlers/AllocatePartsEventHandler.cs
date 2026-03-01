using MediatR;
using Microsoft.EntityFrameworkCore;
using Workshop.Domain.Entities;
using Workshop.Domain.Enums;
using Workshop.Domain.Events;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Handlers;

/// <summary>
/// Reacts to <see cref="QuotationApprovedEvent"/> and handles inventory allocation.
///
/// For every <c>Part</c>-type item in the approved quotation:
/// <list type="bullet">
///   <item>If the part is <b>in stock</b>: deduct the required quantity.</item>
///   <item>If the part is <b>out of stock</b>: create a <see cref="PurchaseNeed"/> record
///         so the purchasing department can procure it from an external supplier.</item>
/// </list>
///
/// Note: this handler runs in the same DI scope as the
/// <see cref="ApproveQuotationCommandHandler"/> and commits its own Unit-of-Work
/// save so stock/purchase data is persisted immediately after the approve event fires.
/// </summary>
public sealed class AllocatePartsEventHandler : INotificationHandler<QuotationApprovedEvent>
{
    private readonly IUnitOfWork _unitOfWork;

    public AllocatePartsEventHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task Handle(QuotationApprovedEvent notification, CancellationToken cancellationToken)
    {
        // 1. Load the approved Quotation including its items.
        var quotations = await _unitOfWork
            .Repository<Quotation>()
            .FindAsync(
                predicate: q => q.Id == notification.QuotationId,
                include: q => q.Include(x => x.Items));

        var quotation = quotations.FirstOrDefault();
        if (quotation is null) return; // Nothing to do if quotation is gone.

        // 2. Filter to Part-type items only.
        var partItems = quotation.Items
            .Where(i => i.Type == QuotationItemType.Part)
            .ToList();

        if (partItems.Count == 0) return;

        foreach (var item in partItems)
        {
            // 3. Try to find a matching Part entity by description (used as the part name/identifier).
            //    A more robust approach would use a PartId FK on QuotationItem — this is a pragmatic
            //    match for the current schema where parts are identified by their description string.
            var parts = await _unitOfWork
                .Repository<Part>()
                .FindAsync(p => p.Name == item.Description);

            var part = parts.FirstOrDefault();

            if (part is not null && part.QuantityInStock >= item.Quantity)
            {
                // IN STOCK: deduct inventory.
                part.QuantityInStock -= item.Quantity;
                _unitOfWork.Repository<Part>().Update(part);
            }
            else
            {
                // OUT OF STOCK: log a PurchaseNeed for the purchasing department.
                var purchaseNeed = new PurchaseNeed(
                    partName: item.Description,
                    quantity: item.Quantity,
                    serviceRequestId: notification.ServiceRequestId);

                await _unitOfWork.Repository<PurchaseNeed>().AddAsync(purchaseNeed);
            }
        }

        // 4. Persist all stock deductions and new PurchaseNeed records.
        await _unitOfWork.SaveChangesAsync();
    }
}
