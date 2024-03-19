namespace Zoo.Services.Abstractions;

/// <summary>
/// Interface wrapper around System.IO.File
/// </summary>
public interface IFile
{
    /// <summary>
    /// Tests whether a file exists. Note that if path describes a directory, Exists will return false.
    /// </summary>
    /// <param name="filePath">path to file</param>
    /// <returns>True if the file given by the specified path exists; otherwise, the result is false.</returns>
    bool Exists(string filePath);

    /// <summary>
    /// Returns the extension of the given path. The returned value includes the period (".") character of the
    /// extension except when you have a terminal period when you get string.Empty, such as ".exe" or ".cpp".
    /// The returned value is null if the given path is null or empty if the given path does not include an
    /// extension.
    /// </summary>
    string GetExtension(string filePath);

    /// <summary>
    /// Asynchronously reads the lines of a file.
    /// </summary>
    /// <param name="filePath">The file to read.</param>
    /// <param name="ct">The token to monitor for cancellation requests. The default value is <see cref="CancellationToken.None"/>.</param>
    /// <returns>The async enumerable that represents all the lines of the file, or the lines that are the result of a query.</returns>
    IAsyncEnumerable<string> ReadLinesAsync(string filePath, CancellationToken ct = default);
}