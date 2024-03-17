using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Zoo.Models.Abstractions;
using Zoo.Models.Enums;

namespace Zoo.Models;

[XmlRoot(ElementName = "Zoo")]
public class Zoo : IZoo, IXmlSerializable
{
    public List<ZooAnimal> Animals { get; set; }
    
    public XmlSchema? GetSchema()
    {
        return (null);
    }

    public void ReadXml(XmlReader reader)
    {
        if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Zoo")
        {
            Animals = new List<ZooAnimal>();
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element &&  Enum.IsDefined(typeof(AnimalType), reader.Name))
                {
                    ZooAnimal animal = new ZooAnimal(reader.Name);
                    animal.Name = reader["name"];
                    animal.Weight = Convert.ToDecimal(reader["kg"]);
                    Animals.Add(animal);
                }
            }
            reader.Read();
        }
    }

    public void WriteXml(XmlWriter writer)
    {
    }
}