using Microsoft.Extensions.Options;
using Serilog;
using Zoo.Common.Infrastructure;
using Zoo.Models;
using Zoo.Services.Abstractions;

namespace Zoo.Services.Implementations;

/// <summary>
/// <inheritdoc cref="IZooPriceService" />
/// </summary>
public class ZooPriceService
    : BaseService, IZooPriceService
{
    private readonly FileLoaderFactory _fileLoaderFactory;
    private readonly IFoodPriceService _foodPriceService;
    private readonly ExternalSettings _externalSettings;

    /// <summary>
    /// Zoo Price service constructor
    /// </summary>
    /// <param name="foodPriceService">injected food price service</param>
    /// <param name="fileLoaderFactory">injected file loader factory service</param>
    /// <param name="externalSettings">external folder configuration object</param>
    /// <param name="logger">logger</param>
    public ZooPriceService(
        IFoodPriceService foodPriceService,
        FileLoaderFactory fileLoaderFactory,
        IOptions<ExternalSettings>  externalSettings,
        ILogger logger)
        : base(logger)
    {
        _foodPriceService = foodPriceService; 
        _fileLoaderFactory = fileLoaderFactory;
        _externalSettings = externalSettings.Value;
    }

    /// <inheritdoc cref="IZooPriceService.CalculatePrice" />
    public async Task<decimal> CalculatePrice(CancellationToken ct)
    {
        const string loggerActionFormat = "{0} to calculate total cost for the zoo";
        logger.Information(loggerActionFormat, "Trying");

        try
        {
            var fileLoaderService = _fileLoaderFactory.GetFileLoaderService(_externalSettings.TextFiles.FileName);
            if ((await fileLoaderService.LoadDataContent($"{_externalSettings.FolderName}/{_externalSettings.TextFiles.FolderName}/{_externalSettings.TextFiles.FileName}", ct)) is not FoodPrices foodPrices)
            {
                return default;
            }

            fileLoaderService = _fileLoaderFactory.GetFileLoaderService(_externalSettings.XmlFiles.FileName);
            if ((await fileLoaderService.LoadDataContent($"{_externalSettings.FolderName}/{_externalSettings.XmlFiles.FolderName}/{_externalSettings.XmlFiles.FileName}", ct)) is not Models.Zoo zooAnimals)
            {
                return default;
            }

            fileLoaderService = _fileLoaderFactory.GetFileLoaderService(_externalSettings.CsvFiles.FileName);
            if ((await fileLoaderService.LoadDataContent($"{_externalSettings.FolderName}/{_externalSettings.CsvFiles.FolderName}/{_externalSettings.CsvFiles.FileName}", ct)) is not AnimalTypeConfig animalTypesData)
            {
                return default;
            }

            decimal sum = 0;

            foreach (var animal in zooAnimals.Animals)
            {
                animal.SetAnimalConfig(animalTypesData.Configurations.FirstOrDefault(a => a.Type == animal.Type));
                sum += _foodPriceService.CalculateAmount(animal, foodPrices);
            }

            logger.Information(loggerActionFormat, "Success");
            return sum;

        }
        catch (Exception ex)
        {
            logger.Error(string.Format(loggerActionFormat, "Failed"), ex);
        }

        return default;
    }
}