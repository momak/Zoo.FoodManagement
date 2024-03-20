namespace Zoo.Models;

public class ZooAnimal : Animal
{
    public ZooAnimal()
    {
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