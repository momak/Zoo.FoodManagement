using Serilog;
using System.Globalization;
using Zoo.Models;
using Zoo.Models.Abstractions;
using Zoo.Models.Enums;
using Zoo.Services.Abstractions;

namespace Zoo.Services.Implementations
{
    /// <summary>
    /// <inheritdoc cref="IFileLoaderService" />
    /// </summary>
    public class TxtFileLoaderService
        : BaseService, IFileLoaderService
    {
        public TxtFileLoaderService(ILogger logger)
            : base(logger)
        { }

        public string Mode() => ".txt";

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
                    var textLines = File.ReadLinesAsync(filePath, ct);
                    var prices = new FoodPrices();

                    await foreach (var line in textLines)
                    {
                        var parts = line.Split("=");
                        prices.Prices.Add(new FoodPrice
                        {
                            Type = Enum.Parse<FoodType>(parts[0]),
                            Price = decimal.Parse(parts[1], CultureInfo.InvariantCulture)
                        });
                    }
                    logger.Information(loggerActionFormat, "Success", filePath);
                    return prices;
                }
            }
            catch (Exception ex)
            {
                logger.Error(string.Format(loggerActionFormat, "Failed", filePath), ex);
            }
            return default;
        }
    }
}
