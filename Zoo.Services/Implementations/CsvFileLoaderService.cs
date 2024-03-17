using CsvHelper;
using CsvHelper.Configuration;
using Serilog;
using System.Globalization;
using Zoo.Models;
using Zoo.Models.Abstractions;
using Zoo.Services.Abstractions;

namespace Zoo.Services.Implementations;

/// <summary>
/// <inheritdoc cref="IFileLoaderService" />
/// </summary>
public class CsvFileLoaderService
    : BaseService, IFileLoaderService
{
    public CsvFileLoaderService(ILogger logger)
        : base(logger)
    { }
    
    public string Mode() => ".csv";

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
                var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HasHeaderRecord = false,
                    Delimiter = ";"
                };

                var animals = new AnimalTypeConfig();

                using var reader = new StreamReader(filePath);
                using var csv = new CsvReader(reader, config);
                
                await foreach (var animal in csv.GetRecordsAsync<Animal>(ct))
                {
                    animals.Configurations.Add(animal);
                }
                
                logger.Information(loggerActionFormat, "Success", filePath);
                return animals;
            }
        }
        catch (Exception ex)
        {
            logger.Error(string.Format(loggerActionFormat, "Failed", filePath), ex);
        }
        return default;
    }
}