using MediatR;
using Workshop.Application.Features.Attachments.Commands;
using Workshop.Application.Interfaces;
using Workshop.Domain.Common;
using Workshop.Domain.Entities;
using Workshop.Domain.Interfaces;

namespace Workshop.Application.Features.Attachments.Handlers;

/// <summary>
/// Handles <see cref="UploadAttachmentCommand"/>:
/// 1. Validates the target <see cref="ServiceRequest"/> exists.
/// 2. Delegates physical file persistence to <see cref="IFileStorageService"/>.
/// 3. Creates and persists an <see cref="Attachment"/> record.
/// 4. Returns the new attachment's ID.
/// </summary>
public sealed class UploadAttachmentCommandHandler
    : IRequestHandler<UploadAttachmentCommand, Result<Guid>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorage;
    private readonly ICurrentUserService _currentUser;

    public UploadAttachmentCommandHandler(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorage,
        ICurrentUserService currentUser)
    {
        _unitOfWork  = unitOfWork;
        _fileStorage = fileStorage;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(
        UploadAttachmentCommand request,
        CancellationToken cancellationToken)
    {
        // ── 1. Verify the service request exists ──────────────────────────────
        var requestRepo = _unitOfWork.Repository<ServiceRequest>();
        var serviceRequest = await requestRepo.GetByIdAsync(request.ServiceRequestId);

        if (serviceRequest is null)
            return Result<Guid>.Failure(
                $"ServiceRequest with ID '{request.ServiceRequestId}' was not found.");

        // ── 2. Persist the file to disk ───────────────────────────────────────
        var (filePath, fileName) = await _fileStorage.SaveAsync(
            request.FileStream,
            request.OriginalFileName,
            request.ContentType,
            cancellationToken);

        // ── 3. Create the domain record ───────────────────────────────────────
        var uploadedBy = _currentUser.GetUserId(); // null for anonymous — safe

        var attachment = new Attachment(
            serviceRequestId: request.ServiceRequestId,
            fileName: fileName,
            filePath: filePath,
            contentType: request.ContentType,
            uploadedBy: uploadedBy);

        var attachmentRepo = _unitOfWork.Repository<Attachment>();
        await attachmentRepo.AddAsync(attachment);
        await _unitOfWork.SaveChangesAsync();

        return Result<Guid>.Success(attachment.Id);
    }
}
