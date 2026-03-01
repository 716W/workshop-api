using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Workshop.Application.Commands;
using Workshop.Application.Handlers;
using Workshop.Application.Interfaces;
using Workshop.Application.Services;

namespace Workshop.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ── Existing services ─────────────────────────────────────────────────
        services.AddScoped<IJobCardService, JobCardService>();
        services.AddScoped<IInventoryService, InventoryService>();

        // ── Factory (Phase 2 - Scenario 1) ───────────────────────────────────
        services.AddScoped<IServiceRequestFactory, ServiceRequestFactory>();

        // ── CQRS Command Handlers (Phase 2 - Scenario 1) ─────────────────────
        services.AddScoped<
            ICommandHandler<CreateServiceRequestCommand, ServiceRequestCreatedResult>,
            CreateServiceRequestCommandHandler>();

        // ── CQRS Command Handlers (Phase 3 - Scenario 2) ─────────────────────
        services.AddScoped<
            ICommandHandler<GenerateQuotationCommand, QuotationGeneratedResult>,
            GenerateQuotationCommandHandler>();

        // ── CQRS Command Handlers (Phase 4 - Scenario 3) ─────────────────────
        services.AddScoped<
            ICommandHandler<ApproveQuotationCommand, ApproveQuotationResult>,
            ApproveQuotationCommandHandler>();

        services.AddScoped<
            ICommandHandler<RejectQuotationCommand, RejectQuotationResult>,
            RejectQuotationCommandHandler>();

        // ── MediatR (Phase 4 - Domain Events) ────────────────────────────────
        // Registers IMediator and auto-discovers all INotificationHandler<T>
        // implementations (e.g. AllocatePartsEventHandler) in this assembly.
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblyContaining<AllocatePartsEventHandler>());

        return services;
    }
}
