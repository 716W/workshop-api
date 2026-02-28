using Workshop.Application.Interfaces;
using Workshop.Domain.Entities;
using Workshop.Domain.Enums;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Services;

public class JobCardService : IJobCardService
{
    private readonly IUnitOfWork _unitOfWork;

    public JobCardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<JobCard> CheckInAsync(Guid vehicleId, string description, decimal estimatedCost)
    {
        var vehicle = await _unitOfWork.Repository<Vehicle>().GetByIdAsync(vehicleId)
            ?? throw new InvalidOperationException($"Vehicle {vehicleId} not found.");

        var jobCard = new JobCard
        {
            Id = Guid.NewGuid(),
            JobNumber = GenerateJobNumber(),
            VehicleId = vehicleId,
            Description = description,
            EstimatedCost = estimatedCost,
            Status = JobCardStatus.CheckedIn,
            CheckedInAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<JobCard>().AddAsync(jobCard);
        await _unitOfWork.SaveChangesAsync();

        return jobCard;
    }

    public async Task<JobCard> StartInspectionAsync(Guid jobCardId, Guid mechanicId)
    {
        var jobCard = await GetAndValidateAsync(jobCardId, JobCardStatus.CheckedIn);

        var mechanic = await _unitOfWork.Repository<Mechanic>().GetByIdAsync(mechanicId)
            ?? throw new InvalidOperationException($"Mechanic {mechanicId} not found.");

        jobCard.MechanicId = mechanicId;
        jobCard.Status = JobCardStatus.Inspection;

        _unitOfWork.Repository<JobCard>().Update(jobCard);
        await _unitOfWork.SaveChangesAsync();

        return jobCard;
    }

    public async Task<JobCard> SubmitInspectionAsync(Guid jobCardId, string inspectionNotes)
    {
        var jobCard = await GetAndValidateAsync(jobCardId, JobCardStatus.Inspection);

        jobCard.InspectionNotes = inspectionNotes;
        jobCard.Status = JobCardStatus.AwaitingApproval;

        _unitOfWork.Repository<JobCard>().Update(jobCard);
        await _unitOfWork.SaveChangesAsync();

        return jobCard;
    }

    public async Task<JobCard> ApproveAsync(Guid jobCardId)
    {
        var jobCard = await GetAndValidateAsync(jobCardId, JobCardStatus.AwaitingApproval);
        return await TransitionAsync(jobCard, JobCardStatus.Approved);
    }

    public async Task<JobCard> StartRepairAsync(Guid jobCardId)
    {
        var jobCard = await GetAndValidateAsync(jobCardId, JobCardStatus.Approved);
        return await TransitionAsync(jobCard, JobCardStatus.InRepair);
    }

    public async Task<JobCard> CompleteRepairAsync(Guid jobCardId)
    {
        var jobCard = await GetAndValidateAsync(jobCardId, JobCardStatus.InRepair);
        return await TransitionAsync(jobCard, JobCardStatus.QualityCheck);
    }

    public async Task<JobCard> PassQualityCheckAsync(Guid jobCardId)
    {
        var jobCard = await GetAndValidateAsync(jobCardId, JobCardStatus.QualityCheck);

        // Calculate final cost from consumed parts
        var parts = await _unitOfWork.Repository<JobCardPart>()
            .FindAsync(jp => jp.JobCardId == jobCardId);
        jobCard.FinalCost = parts.Sum(p => p.Quantity * p.UnitPriceAtTime);

        return await TransitionAsync(jobCard, JobCardStatus.ReadyForInvoice);
    }

    public async Task<JobCard> FailQualityCheckAsync(Guid jobCardId, string reason)
    {
        var jobCard = await GetAndValidateAsync(jobCardId, JobCardStatus.QualityCheck);

        jobCard.InspectionNotes += $"\n[QC FAILED]: {reason}";

        return await TransitionAsync(jobCard, JobCardStatus.InRepair);
    }

    public async Task<JobCard> MarkInvoicedAsync(Guid jobCardId)
    {
        var jobCard = await GetAndValidateAsync(jobCardId, JobCardStatus.ReadyForInvoice);
        return await TransitionAsync(jobCard, JobCardStatus.Invoiced);
    }

    public async Task<JobCard> ReleaseAsync(Guid jobCardId)
    {
        var jobCard = await GetAndValidateAsync(jobCardId, JobCardStatus.Invoiced);

        jobCard.CompletedAt = DateTime.UtcNow;

        return await TransitionAsync(jobCard, JobCardStatus.Released);
    }

    public async Task<JobCard?> GetByIdAsync(Guid jobCardId)
    {
        return await _unitOfWork.Repository<JobCard>().GetByIdAsync(jobCardId);
    }

    public async Task<IEnumerable<JobCard>> GetAllAsync()
    {
        return await _unitOfWork.Repository<JobCard>().GetAllAsync();
    }

    public async Task<IEnumerable<JobCard>> GetByStatusAsync(JobCardStatus status)
    {
        return await _unitOfWork.Repository<JobCard>()
            .FindAsync(j => j.Status == status);
    }

    // ── Private Helpers ─────────────────────────────────────────

    private async Task<JobCard> GetAndValidateAsync(Guid jobCardId, JobCardStatus expectedStatus)
    {
        var jobCard = await _unitOfWork.Repository<JobCard>().GetByIdAsync(jobCardId)
            ?? throw new InvalidOperationException($"JobCard {jobCardId} not found.");

        if (jobCard.Status != expectedStatus)
            throw new InvalidOperationException(
                $"JobCard {jobCardId} is in '{jobCard.Status}' state. Expected '{expectedStatus}'.");

        return jobCard;
    }

    private async Task<JobCard> TransitionAsync(JobCard jobCard, JobCardStatus newStatus)
    {
        jobCard.Status = newStatus;

        _unitOfWork.Repository<JobCard>().Update(jobCard);
        await _unitOfWork.SaveChangesAsync();

        return jobCard;
    }

    private static string GenerateJobNumber()
    {
        return $"JOB-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
    }
}
