namespace Workshop.API.Routes;

/// <summary>
/// Central registry of all API route constants.
/// Controllers MUST reference these constants instead of hardcoding strings,
/// ensuring a single place to update route shapes without hunting across files.
/// </summary>
/// <remarks>
/// Naming convention:
///   - <c>Base</c>   → The [Route] attribute value for the controller class.
///   - Other fields  → Relative route templates used on individual action methods.
/// </remarks>
public static class ApiRoutes
{
    // ── Reception ─────────────────────────────────────────────────────────────

    /// <summary>Routes for <c>ReceptionController</c>.</summary>
    public static class Reception
    {
        /// <summary>Controller-level base route: <c>api/reception</c>.</summary>
        public const string Base = "api/reception";

        /// <summary>POST  api/reception/create — log a new customer service request.</summary>
        public const string Create = "create";

        /// <summary>POST  api/requests/{id}/release — close and release the vehicle.</summary>
        public const string Release = "/api/requests/{id:guid}/release";
    }

    // ── Operations ────────────────────────────────────────────────────────────

    /// <summary>Routes for <c>OperationsController</c>.</summary>
    public static class Operations
    {
        /// <summary>Controller-level base route: <c>api/requests</c>.</summary>
        public const string Base = "api/requests";

        /// <summary>GET  api/requests — paginated list of service requests.</summary>
        public const string GetPaged = "";

        /// <summary>GET  api/requests/{id} — full details of a specific service request.</summary>
        public const string GetById = "{id:guid}";

        /// <summary>POST  api/requests/{id}/quotations — generate a quotation for a service request.</summary>
        public const string GenerateQuotation = "{id:guid}/quotations";

        /// <summary>PATCH  api/requests/{id}/status — update the service request status.</summary>
        public const string UpdateStatus = "{id:guid}/status";

        /// <summary>POST  api/requests/{id}/qc — perform quality control check.</summary>
        public const string PerformQC = "{id:guid}/qc";
    }

    // ── Quotations ────────────────────────────────────────────────────────────

    /// <summary>Routes for <c>QuotationsController</c>.</summary>
    public static class Quotations
    {
        /// <summary>Controller-level base route: <c>api/quotations</c>.</summary>
        public const string Base = "api/quotations";

        /// <summary>POST  api/quotations/{id}/approve — approve a pending quotation.</summary>
        public const string Approve = "{id:guid}/approve";

        /// <summary>POST  api/quotations/{id}/reject — reject a pending quotation.</summary>
        public const string Reject = "{id:guid}/reject";
    }

    // ── Billing ───────────────────────────────────────────────────────────────

    /// <summary>Routes for <c>BillingController</c>.</summary>
    public static class Billing
    {
        /// <summary>Controller-level base route: <c>api</c>.</summary>
        public const string Base = "api";

        /// <summary>POST  api/requests/{id}/invoice — generate invoice for request.</summary>
        public const string GenerateInvoice = "requests/{id:guid}/invoice";

        /// <summary>POST  api/invoices/{id}/pay — pay a specific invoice.</summary>
        public const string PayInvoice = "invoices/{id:guid}/pay";

        /// <summary>GET  api/invoices/{id} — get full details of an invoice.</summary>
        public const string GetById = "invoices/{id:guid}";
    }

    // ── Workers ───────────────────────────────────────────────────────────────

    /// <summary>Routes for <c>WorkersController</c>.</summary>
    public static class Workers
    {
        /// <summary>Controller-level base route: <c>api/workers</c>.</summary>
        public const string Base = "api/workers";

        /// <summary>GET  api/workers/{id}/commissions — get paginated commissions of a worker.</summary>
        public const string GetCommissions = "{id:guid}/commissions";
    }

    // ── Inventory ─────────────────────────────────────────────────────────────

    /// <summary>Routes for <c>InventoryController</c>.</summary>
    public static class Inventory
    {
        /// <summary>Controller-level base route: <c>api/inventory</c>.</summary>
        public const string Base = "api/[controller]";

        /// <summary>GET/POST  api/inventory — list or add parts.</summary>
        public const string List = "";

        /// <summary>GET  api/inventory/{id} — get part by ID.</summary>
        public const string GetById = "{id:guid}";

        /// <summary>GET  api/inventory/low-stock — parts below threshold.</summary>
        public const string LowStock = "low-stock";

        /// <summary>POST  api/inventory/{partId}/consume — consume part stock.</summary>
        public const string Consume = "{partId:guid}/consume";

        /// <summary>POST  api/inventory/{partId}/restock — restock a part.</summary>
        public const string Restock = "{partId:guid}/restock";
    }

    // ── JobCards ──────────────────────────────────────────────────────────────

    /// <summary>Routes for <c>JobCardsController</c>.</summary>
    public static class JobCards
    {
        /// <summary>Controller-level base route: <c>api/jobcards</c>.</summary>
        public const string Base = "api/[controller]";

        public const string GetById        = "{id:guid}";
        public const string GetByStatus    = "status/{status}";
        public const string CheckIn        = "checkin";
        public const string StartInspect   = "{id:guid}/inspect";
        public const string SubmitInspect  = "{id:guid}/inspect/submit";
        public const string Approve        = "{id:guid}/approve";
        public const string StartRepair    = "{id:guid}/repair/start";
        public const string CompleteRepair = "{id:guid}/repair/complete";
        public const string QcPass         = "{id:guid}/qc/pass";
        public const string QcFail         = "{id:guid}/qc/fail";
        public const string Invoice        = "{id:guid}/invoice";
        public const string Release        = "{id:guid}/release";
    }
}
