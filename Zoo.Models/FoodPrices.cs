using Zoo.Models.Abstractions;

namespace Zoo.Models;

public class FoodPrices : ZooBase
{
    public List<FoodPrice> Prices = new();
}