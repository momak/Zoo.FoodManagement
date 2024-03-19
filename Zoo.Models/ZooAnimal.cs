using Zoo.Models.Enums;

namespace Zoo.Models;

public class ZooAnimal : Animal
{
    public ZooAnimal()
    {
    }

    public ZooAnimal(string type)
    {
        Type = Enum.Parse<AnimalType>(type);
    }

    public string Name { get; set; }

    public decimal Weight { get; set; }


    public void SetAnimalConfig(Animal? animal)
    {
        if (animal == null) return;

        FoodWeightRate = animal.FoodWeightRate;
        EatingType = animal.EatingType;
        MeatRate = animal.MeatRate;
    }
}