using Workshop.Domain.Entities;
using Workshop.Domain.Enums;

namespace Workshop.Application.Interfaces;

public interface IJobCardService
{
    Task<JobCard> CheckInAsync(Guid vehicleId, string description, decimal estimatedCost);
    Task<JobCard> StartInspectionAsync(Guid jobCardId, Guid mechanicId);
    Task<JobCard> SubmitInspectionAsync(Guid jobCardId, string inspectionNotes);
    Task<JobCard> ApproveAsync(Guid jobCardId);
    Task<JobCard> StartRepairAsync(Guid jobCardId);
    Task<JobCard> CompleteRepairAsync(Guid jobCardId);
    Task<JobCard> PassQualityCheckAsync(Guid jobCardId);
    Task<JobCard> FailQualityCheckAsync(Guid jobCardId, string reason);
    Task<JobCard> MarkInvoicedAsync(Guid jobCardId);
    Task<JobCard> ReleaseAsync(Guid jobCardId);
    Task<JobCard?> GetByIdAsync(Guid jobCardId);
    Task<IEnumerable<JobCard>> GetAllAsync();
    Task<IEnumerable<JobCard>> GetByStatusAsync(JobCardStatus status);
}
