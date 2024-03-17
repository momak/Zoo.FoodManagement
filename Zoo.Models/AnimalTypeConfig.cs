using Zoo.Models.Abstractions;

namespace Zoo.Models;

public class AnimalTypeConfig : IZoo
{
    public List<Animal> Configurations { get; set; } = new();
}