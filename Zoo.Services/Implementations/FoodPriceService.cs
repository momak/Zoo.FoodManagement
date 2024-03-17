using Serilog;
using Zoo.Common.Extensions;
using Zoo.Models;
using Zoo.Models.Enums;
using Zoo.Services.Abstractions;

namespace Zoo.Services.Implementations;

/// <summary>
/// <inheritdoc cref="IFoodPriceService" />
/// </summary>
public class FoodPriceService
    : BaseService, IFoodPriceService
{
    public FoodPriceService(ILogger logger)
        : base(logger)
    {
    }

    /// <inheritdoc cref="IFoodPriceService.CalculateAmount" />
    public decimal CalculateAmount(ZooAnimal animal, FoodPrices prices)
    {
        if (animal == null) throw new ArgumentNullException(nameof(animal));
        if (prices == null) throw new ArgumentNullException(nameof(prices));

        const string loggerActionFormat = "{0} to calculate food cost amount for animal with Name '{1}'";
        logger.Information(loggerActionFormat, "Trying", animal.Name);

        try
        {
            var amountFood = animal.Weight * animal.FoodWeightRate;
            decimal price = 0;
            if (animal.EatingType == EatingType.both)
            {
                price = amountFood.Value * animal.MeatRate.FromPercentageString() * prices.Prices.FirstOrDefault(x => x.Type == FoodType.Meat).Price +
                       amountFood.Value * (1 - animal.MeatRate.FromPercentageString()) * prices.Prices.FirstOrDefault(x => x.Type == FoodType.Fruit).Price;

                logger.Information(loggerActionFormat, "Success", animal.Name);
                return price;
            }

            price = amountFood.Value * prices.Prices.FirstOrDefault(x => (int)x.Type == (int)animal.EatingType).Price;

            logger.Information(loggerActionFormat, "Success", animal.Name);
            return price;

        }
        catch (Exception ex)
        {
            logger.Error(string.Format(loggerActionFormat, "Failed", animal.Name), ex);
        }

        return default;
    }
}