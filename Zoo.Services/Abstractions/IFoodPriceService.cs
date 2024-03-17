using Zoo.Models;

namespace Zoo.Services.Abstractions;

public interface IFoodPriceService
{
    /// <summary>
    /// Calculate the total food price per day for a zoo animal
    /// </summary>
    /// <param name="animal"></param>
    /// <param name="prices"></param>
    /// <returns>total amount per day for the animal</returns>
    decimal CalculateAmount(ZooAnimal animal, FoodPrices prices);
}