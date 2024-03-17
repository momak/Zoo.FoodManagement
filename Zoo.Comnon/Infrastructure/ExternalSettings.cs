namespace Zoo.Common.Infrastructure;

public class ExternalSettings : PathNames
{
    public TextFiles TextFiles { get; set; }
    public XmlFiles XmlFiles { get; set; }
    public CsvFiles CsvFiles { get; set; }
}