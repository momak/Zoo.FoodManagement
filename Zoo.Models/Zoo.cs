using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Zoo.Common.Extensions;
using Zoo.Models.Abstractions;
using Zoo.Models.Enums;

namespace Zoo.Models;

[XmlRoot(ElementName = "Zoo")]
public class Zoo : ZooBase, IXmlSerializable
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
                    ZooAnimal animal = new ZooAnimal()
                    {
                        Name = reader["name"]??string.Empty,
                        Weight = Convert.ToDecimal(reader["kg"]),
                        Type = reader.Name.ToEnum<AnimalType>()
                    };
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