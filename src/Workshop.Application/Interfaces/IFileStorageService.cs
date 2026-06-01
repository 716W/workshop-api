namespace Workshop.Application.Interfaces;

/// <summary>
/// Abstracts the physical file-storage mechanism so the Application layer
/// remains framework-agnostic.  The controller unpacks <c>IFormFile</c> and
/// passes raw stream + metadata here.
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Persists a file stream and returns its stored location.
    /// </summary>
    /// <param name="fileStream">Raw byte stream of the uploaded file.</param>
    /// <param name="originalFileName">
    ///   File name supplied by the client. Used to derive the stored name and
    ///   preserve the original extension.
    /// </param>
    /// <param name="contentType">MIME type (e.g. <c>image/jpeg</c>).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    ///   A tuple of:
    ///   <list type="bullet">
    ///     <item><description><c>FilePath</c> – relative server path (e.g. <c>uploads/abc_photo.jpg</c>).</description></item>
    ///     <item><description><c>FileName</c> – the actual name saved on disk.</description></item>
    ///   </list>
    /// </returns>
    Task<(string FilePath, string FileName)> SaveAsync(
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken ct = default);
}
