using Zoo.Services.Abstractions;

namespace Zoo.Services.Implementations;

/// <summary>
/// <inheritdoc cref="IFile" />
/// </summary>
public class FileWrapper : IFile
{
    /// <inheritdoc cref="IFile.Exists" />
    public bool Exists(string filePath)
    {
        return File.Exists(filePath);
    }

    /// <inheritdoc cref="IFile.ReadLinesAsync" />
    public async IAsyncEnumerable<string> ReadLinesAsync(string filePath, CancellationToken ct = default)
    {
        await foreach (var line in File.ReadLinesAsync(filePath, ct)) yield return line;
    }

    /// <inheritdoc cref="IFile.GetExtension" />
    public string GetExtension(string filePath)
    {
        return Path.GetExtension(filePath);
    }
}