using Bogus;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
using System.ComponentModel;
using System.Diagnostics;
using Zoo.Common.Infrastructure;
using Zoo.Models;
using Zoo.Models.Enums;
using Zoo.Services.Abstractions;
using Zoo.Services.Implementations;

namespace Zoo.Services.Test.ZooPriceService;

public class CalculatePriceTests
{
    private readonly Mock<IFoodPriceService> _foodPriceServiceMock;
    private readonly Mock<FileLoaderFactory> _fileLoaderFactoryMock;
    private readonly Mock<IFileLoaderService> _textFileLoaderServiceMock;
    private readonly Mock<IFileLoaderService> _xmlFileLoaderServiceMock;
    private readonly Mock<IFileLoaderService> _csvFileLoaderServiceMock;
    private readonly IOptions<ExternalSettings> _externalSettings;
    private readonly Mock<ILogger> _loggerMock;
    private readonly CancellationToken _token;

    private readonly Implementations.ZooPriceService _service;

    public CalculatePriceTests()
    {
        _textFileLoaderServiceMock = new Mock<IFileLoaderService>();
        _textFileLoaderServiceMock.Setup(x=>x.Mode()).Returns(() => ".txt");
        _xmlFileLoaderServiceMock = new Mock<IFileLoaderService>();
        _xmlFileLoaderServiceMock.Setup(x => x.Mode()).Returns(() => ".xml");
        _csvFileLoaderServiceMock = new Mock<IFileLoaderService>();
        _csvFileLoaderServiceMock.Setup(x => x.Mode()).Returns(() => ".csv");

        _foodPriceServiceMock = new Mock<IFoodPriceService>();
        _fileLoaderFactoryMock = new Mock<FileLoaderFactory>(MockBehavior.Strict, new object[]{ new List<IFileLoaderService>{ _textFileLoaderServiceMock.Object, _xmlFileLoaderServiceMock.Object, _csvFileLoaderServiceMock.Object } });
        _externalSettings = Options.Create(new ExternalSettings()
        {
            FolderName = "ExternalTest",
            FileName = string.Empty,
            TextFiles = new TextFiles()
            {
                FolderName = "Txt",
                FileName = "someTextFile.txt"
            },
            XmlFiles = new XmlFiles()
            {
                FolderName = "Xml",
                FileName = "someXmlFile.xml"
            },
            CsvFiles = new CsvFiles()
            {
                FolderName = "Csv",
                FileName = "someCsvFile.csv"
            }
        });
        _loggerMock = new Mock<ILogger>();
        _token = CancellationToken.None;
        

        _service = new Implementations.ZooPriceService(_foodPriceServiceMock.Object, _fileLoaderFactoryMock.Object, _externalSettings, _loggerMock.Object);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(ZooPriceService))]
    public async Task Should_CalculatePrice_When_DataCorrect()
    {
        // Arrange
        _textFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => new FoodPrices()
            {
                Prices = new List<FoodPrice>()
                {
                    new() {Price = 1,Type = FoodType.Fruit },
                    new() {Price = 2,Type = FoodType.Meat }
                }
            });
        _xmlFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => new Models.Zoo()
            {
                Animals = new List<ZooAnimal>()
                {
                    new()
                    {
                        Name = "Li",
                        Weight = 100,
                        Type = AnimalType.Lion
                    },
                    new()
                    {
                        Name = "Xebra",
                        Weight = 70,
                        Type = AnimalType.Zebra
                    },
                    new()
                    {
                        Name = "Pir",
                        Weight = 0.4m,
                        Type = AnimalType.Piranha
                    }
                }
            });
        _csvFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => new AnimalTypeConfig()
            {
                Configurations = new List<Animal>()
                {
                    new()
                    {
                        Type = AnimalType.Lion,
                        FoodWeightRate = 0.10m,
                        EatingType = EatingType.meat
                    },
                    new()
                    {
                        Type = AnimalType.Zebra,
                        FoodWeightRate = 0.08m,
                        EatingType = EatingType.fruit
                    },
                    new()
                    {
                        Type = AnimalType.Piranha,
                        MeatRate = "50%",
                        FoodWeightRate = 0.5m,
                        EatingType = EatingType.both
                    }
                }
            });

        // Act
        var actual = await _service.CalculatePrice(_token);

        // Assert
        actual.Equals(1);
    }
}