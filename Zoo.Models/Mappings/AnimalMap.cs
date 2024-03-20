using CsvHelper.Configuration;

namespace Zoo.Models.Mappings
{
    public sealed class AnimalMap : ClassMap<Animal>
    {
        public AnimalMap()
        {
            Map(m => m.Type).Index(0);
            Map(m => m.FoodWeightRate).Index(1);
            Map(m => m.EatingType).Index(2); 
            Map(m => m.MeatRate).Index(3);
        }
    }
}
