using Zoo.Models.Abstractions;

namespace Zoo.Models;

public class AnimalTypeConfig : ZooBase
{
    public List<Animal> Configurations { get; set; } = new();
}