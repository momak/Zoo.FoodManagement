using Zoo.Models.Abstractions;

namespace Zoo.Services.Abstractions;

public interface IFileLoaderService
{
    string Mode();

    /// <summary>
    /// Load Contents of file and maps to object
    /// </summary>
    /// <param name="filePath">path to file</param>
    /// <param name="ct">Cancellation Token</param>
    /// <returns>model representation of file contents</returns>
    Task<IZoo> LoadDataContent(string filePath, CancellationToken ct);
}