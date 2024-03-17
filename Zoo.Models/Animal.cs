using CsvHelper.Configuration.Attributes;
using Zoo.Models.Enums;

namespace Zoo.Models;

public class Animal
{
    [Index(0)]
    public AnimalType? Type { get; set; }

    [Index(2)]
    public EatingType? EatingType { get; set; }

    [Index(1)]
    public decimal? FoodWeightRate { get; set; }

    [Index(3)]
    public string MeatRate { get; set; }
}