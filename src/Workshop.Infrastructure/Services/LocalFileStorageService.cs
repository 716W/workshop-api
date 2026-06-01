using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Workshop.Application.Interfaces;

namespace Workshop.Infrastructure.Services;

/// <summary>
/// Saves uploaded files to the <c>wwwroot/uploads</c> directory.
/// Each file is stored under a <c>{Guid}_{sanitisedOriginalName}</c> pattern
/// to guarantee uniqueness and prevent collisions or path-traversal attacks.
/// </summary>
public sealed class LocalFileStorageService : IFileStorageService
{
    private const string UploadsFolder = "uploads";
    private readonly string _uploadsRoot;

    public LocalFileStorageService(IWebHostEnvironment env)
    {
        // WebRootPath is wwwroot/; fall back to ContentRootPath if it's null (e.g. test hosts).
        var webRoot = env.WebRootPath ?? env.ContentRootPath;
        _uploadsRoot = Path.Combine(webRoot, UploadsFolder);
    }

    /// <inheritdoc />
    public async Task<(string FilePath, string FileName)> SaveAsync(
        Stream fileStream,
        string originalFileName,
        string contentType,
        CancellationToken ct = default)
    {
        // Ensure the uploads directory exists.
        Directory.CreateDirectory(_uploadsRoot);

        // Sanitise the original name and prepend a GUID to guarantee uniqueness.
        var sanitised = Path.GetFileName(originalFileName); // strips directory traversal
        var storedName = $"{Guid.NewGuid():N}_{sanitised}";
        var fullPath = Path.Combine(_uploadsRoot, storedName);

        await using var destination = new FileStream(
            fullPath, FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 81920, useAsync: true);

        await fileStream.CopyToAsync(destination, ct);

        // Return the relative path so the controller can build a public URL.
        var relativePath = $"{UploadsFolder}/{storedName}";
        return (relativePath, storedName);
    }
}
