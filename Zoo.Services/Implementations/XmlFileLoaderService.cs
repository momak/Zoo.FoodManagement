using Serilog;
using System.Xml.Serialization;
using Zoo.Models.Abstractions;
using Zoo.Services.Abstractions;

namespace Zoo.Services.Implementations;

/// <summary>
/// <inheritdoc cref="IFileLoaderService" />
/// </summary>
public class XmlFileLoaderService
    : BaseService, IFileLoaderService
{
    public XmlFileLoaderService(ILogger logger)
        : base(logger)
    {
    }

    public string Mode() => ".xml";

    /// <inheritdoc cref="IFileLoaderService.LoadDataContent" />
    public async Task<IZoo> LoadDataContent(string filePath, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(filePath)) return default;

        const string loggerActionFormat = "{0} to load data from '{1}'";
        logger.Information(loggerActionFormat, "Trying", filePath);
        
        try
        {
            if (File.Exists(filePath))
            {
                var serializer = new XmlSerializer(typeof(Models.Zoo));

                using var reader = new StreamReader(filePath);
                var zoo = serializer.Deserialize(reader) as Models.Zoo;

                logger.Information(loggerActionFormat, "Success", filePath);
                return await Task.FromResult(zoo);
            }
        }
        catch (Exception ex)
        {
            logger.Error(string.Format(loggerActionFormat, "Failed", filePath), ex);
        }

        return default;
    }
}