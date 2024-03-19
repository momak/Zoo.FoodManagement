using Bogus;
using Microsoft.Extensions.Options;
using Moq;
using Serilog;
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
        //Default setup of all tests
        _textFileLoaderServiceMock = new Mock<IFileLoaderService>();
        _textFileLoaderServiceMock.Setup(x => x.Mode()).Returns(() => ".txt");
        _xmlFileLoaderServiceMock = new Mock<IFileLoaderService>();
        _xmlFileLoaderServiceMock.Setup(x => x.Mode()).Returns(() => ".xml");
        _csvFileLoaderServiceMock = new Mock<IFileLoaderService>();
        _csvFileLoaderServiceMock.Setup(x => x.Mode()).Returns(() => ".csv");

        _foodPriceServiceMock = new Mock<IFoodPriceService>();
        _fileLoaderFactoryMock = new Mock<FileLoaderFactory>(MockBehavior.Strict, new object[] { new List<IFileLoaderService> { _textFileLoaderServiceMock.Object, _xmlFileLoaderServiceMock.Object, _csvFileLoaderServiceMock.Object } });
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
        var animals = new List<ZooAnimal>()
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
        };
        var foodPrices = new FoodPrices()
        {
            Prices = new List<FoodPrice>()
            {
                new() { Price = 1, Type = FoodType.Fruit },
                new() { Price = 2, Type = FoodType.Meat }
            }
        };

        _textFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => foodPrices);
        _xmlFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => new Models.Zoo() { Animals = animals });
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
        _foodPriceServiceMock.Setup(x => x.CalculateAmount(animals.First(), foodPrices))
            .Returns(() => 20m);
        _foodPriceServiceMock.Setup(x => x.CalculateAmount(animals.Skip(1).Take(1).First(), foodPrices))
            .Returns(() => 5.6m);
        _foodPriceServiceMock.Setup(x => x.CalculateAmount(animals.Last(), foodPrices))
            .Returns(() => 0.3m);

        // Act
        var actual = await _service.CalculatePrice(_token);

        // Assert
        actual.Equals(20 + 5.6 + 0.3);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(ZooPriceService))]
    public async Task Should_CalculatePrice_As_0_When_Txt_Service_fails()
    {
        // Arrange
        var animals = new Faker<ZooAnimal>()
            .RuleFor(a => a.Name, f => f.Name.FirstName())
            .RuleFor(a => a.Weight, f => f.Random.Decimal(0.2m, 200m))
            .Generate(10);
        var animalConfig = new Faker<AnimalTypeConfig>()
            .RuleFor(a => a.Configurations, f => new Faker<Animal>()
                .Generate(3))
            .Generate();
        _textFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => null)
            .Verifiable();
        _xmlFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => new Models.Zoo() { Animals = animals })
            .Verifiable();
        _csvFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => animalConfig)
            .Verifiable();
        _foodPriceServiceMock.Setup(x => x.CalculateAmount(It.IsAny<ZooAnimal>(), It.IsAny<FoodPrices>()))
            .Returns(() => 20m);

        // Act
        var actual = await _service.CalculatePrice(_token);

        // Assert
        actual.Equals(default);
        _textFileLoaderServiceMock.Verify(x => x.LoadDataContent(It.IsAny<string>(), _token), Times.Once);
        _xmlFileLoaderServiceMock.Verify(x => x.LoadDataContent(It.IsAny<string>(), _token), Times.Never);
        _csvFileLoaderServiceMock.Verify(x => x.LoadDataContent(It.IsAny<string>(), _token), Times.Never);
        _foodPriceServiceMock.Verify(x => x.CalculateAmount(It.IsAny<ZooAnimal>(), It.IsAny<FoodPrices>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(ZooPriceService))]
    public async Task Should_CalculatePrice_As_0_When_Xml_Service_fails()
    {
        // Arrange
        var foodPrices = new Faker<FoodPrices>()
            .RuleFor(fps => fps.Prices, f => new Faker<FoodPrice>()
                .RuleFor(fp => fp.Price, f => f.Random.Decimal(0m, 9999m))
                .RuleFor(fp => fp.Type, f => f.PickRandom<FoodType>())
                .Generate(2))
            .Generate();
        var animalConfig = new Faker<AnimalTypeConfig>()
            .RuleFor(a => a.Configurations, f => new Faker<Animal>()
                .Generate(3))
            .Generate();

        _textFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => foodPrices)
            .Verifiable();
        _xmlFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => null)
            .Verifiable();
        _csvFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => animalConfig)
            .Verifiable();
        _foodPriceServiceMock.Setup(x => x.CalculateAmount(It.IsAny<ZooAnimal>(), It.IsAny<FoodPrices>()))
            .Returns(() => 20m)
            .Verifiable();

        // Act
        var actual = await _service.CalculatePrice(_token);

        // Assert
        actual.Equals(default);
        _textFileLoaderServiceMock.Verify(x => x.LoadDataContent(It.IsAny<string>(), _token), Times.Once);
        _xmlFileLoaderServiceMock.Verify(x => x.LoadDataContent(It.IsAny<string>(), _token), Times.Once);
        _csvFileLoaderServiceMock.Verify(x => x.LoadDataContent(It.IsAny<string>(), _token), Times.Never);
        _foodPriceServiceMock.Verify(x => x.CalculateAmount(It.IsAny<ZooAnimal>(), It.IsAny<FoodPrices>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(ZooPriceService))]
    public async Task Should_CalculatePrice_As_0_When_Csv_Service_fails()
    {
        // Arrange
        var foodPrices = new Faker<FoodPrices>()
            .RuleFor(fps => fps.Prices, f => new Faker<FoodPrice>()
                .RuleFor(fp => fp.Price, f => f.Random.Decimal(0m, 9999m))
                .RuleFor(fp => fp.Type, f => f.PickRandom<FoodType>())
                .Generate(2))
            .Generate();
        var animals = new Faker<ZooAnimal>()
            .RuleFor(a => a.Name, f => f.Name.FirstName())
            .RuleFor(a => a.Weight, f => f.Random.Decimal(0.2m, 200m))
            .Generate(10);

        _textFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => foodPrices)
            .Verifiable();
        _xmlFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(new Models.Zoo() { Animals = animals })
            .Verifiable();
        _csvFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .ReturnsAsync(() => null)
            .Verifiable();
        _foodPriceServiceMock.Setup(x => x.CalculateAmount(It.IsAny<ZooAnimal>(), It.IsAny<FoodPrices>()))
            .Returns(() => 20m)
            .Verifiable();

        // Act
        var actual = await _service.CalculatePrice(_token);

        // Assert
        actual.Equals(default);
        _textFileLoaderServiceMock.Verify(x => x.LoadDataContent(It.IsAny<string>(), _token), Times.Once);
        _xmlFileLoaderServiceMock.Verify(x => x.LoadDataContent(It.IsAny<string>(), _token), Times.Once);
        _csvFileLoaderServiceMock.Verify(x => x.LoadDataContent(It.IsAny<string>(), _token), Times.Once);
        _foodPriceServiceMock.Verify(x => x.CalculateAmount(It.IsAny<ZooAnimal>(), It.IsAny<FoodPrices>()), Times.Never);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(ZooPriceService))]
    public async Task Should_CalculatePrice_As_0_When_Exception_Is_Thrown()
    {
        // Arrange
        var exception = new Exception();
        _textFileLoaderServiceMock.Setup(x => x.LoadDataContent(It.IsAny<string>(), _token))
            .Callback(() => throw exception);
        
        // Act
        var actual = await _service.CalculatePrice(_token);

        // Assert
        actual.Equals(default);
        _textFileLoaderServiceMock.Verify(x => x.LoadDataContent(It.IsAny<string>(), _token), Times.Once);
        _loggerMock.Verify(x => x.Information("{0} to calculate total cost for the zoo", "Trying"), Times.Once);
        _loggerMock.Verify(x => x.Error($"Failed to calculate total cost for the zoo", exception), Times.Once);
    }

}
