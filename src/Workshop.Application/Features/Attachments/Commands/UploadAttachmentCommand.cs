using MediatR;
using Workshop.Domain.Common;

namespace Workshop.Application.Features.Attachments.Commands;

/// <summary>
/// Command to upload a file and associate it with an existing <see cref="ServiceRequest"/>.
/// The controller is responsible for unpacking <c>IFormFile</c> into a raw stream
/// before dispatching this command, keeping this record framework-agnostic.
/// </summary>
/// <param name="ServiceRequestId">The service request to attach the file to.</param>
/// <param name="FileStream">Raw byte stream of the uploaded file.</param>
/// <param name="OriginalFileName">The file name supplied by the client.</param>
/// <param name="ContentType">MIME type of the file (e.g. <c>image/jpeg</c>).</param>
public record UploadAttachmentCommand(
    Guid ServiceRequestId,
    Stream FileStream,
    string OriginalFileName,
    string ContentType) : IRequest<Result<Guid>>;
