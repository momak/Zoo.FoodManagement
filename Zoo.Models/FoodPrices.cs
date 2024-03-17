using Zoo.Models.Abstractions;

namespace Zoo.Models;

public class FoodPrices : IZoo
{
    public List<FoodPrice> Prices = new();
}