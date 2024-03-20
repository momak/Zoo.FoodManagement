using Zoo.Models.Enums;

namespace Zoo.Models;

public class Animal
{
    public AnimalType? Type { get; set; }

    public EatingType? EatingType { get; set; }

    public decimal? FoodWeightRate { get; set; }

    public string MeatRate { get; set; }
}