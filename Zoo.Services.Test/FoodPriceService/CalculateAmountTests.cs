using Bogus;
using Moq;
using Serilog;
using Zoo.Models;
using Zoo.Models.Enums;

namespace Zoo.Services.Test.FoodPriceService
{
    public class CalculateAmountTests
    {
        private readonly Mock<ILogger> _loggerMock;

        private readonly Implementations.FoodPriceService _service;

        public CalculateAmountTests()
        {
            _loggerMock = new Mock<ILogger>();
            _service = new Implementations.FoodPriceService(_loggerMock.Object);
        }

        [Theory]
        [Trait("Category", "Service")]
        [Trait("Category", nameof(Implementations.FoodPriceService))]
        [InlineData(20, FoodType.Meat, EatingType.meat)]
        [InlineData(10, FoodType.Fruit, EatingType.fruit)]
        public void Should_CalculateAmount_For_Correct_Input(decimal price, FoodType foodType, EatingType eatingType)
        {
            // Arrange
            var foodPrices = new FoodPrices()
            {
                Prices = new List<FoodPrice>()
                {
                    new ()
                    {
                        Price = price,
                        Type = foodType
                    }
                }
            };
            var animal = new Faker<ZooAnimal>()
                .RuleFor(a => a.Name, f => f.Name.FirstName())
                .RuleFor(a => a.Weight, f => f.Random.Decimal(0.1m, 200))
                .RuleFor(a => a.FoodWeightRate, f => f.Random.Decimal())
                .RuleFor(a => a.EatingType, eatingType)
                .Generate();
            var expected = animal.Weight * animal.FoodWeightRate * price;

            // Act
            var actual = _service.CalculateAmount(animal, foodPrices);

            // Assert
            actual.Equals(expected);
        }


        [Fact]
        [Trait("Category", "Service")]
        [Trait("Category", nameof(Implementations.FoodPriceService))]
        public void Should_CalculateAmount_For_Both_Input()
        {
            // Arrange
            var foodPrices = new FoodPrices()
            {
                Prices = new List<FoodPrice>()
                {
                    new () { Price = 10, Type = FoodType.Meat },
                    new () { Price = 7, Type = FoodType.Fruit }
                }
            };
            var animal = new Faker<ZooAnimal>()
                .RuleFor(a => a.Name, f => f.Name.FirstName())
                .RuleFor(a => a.Weight, 70m)
                .RuleFor(a => a.FoodWeightRate, 0.6m)
                .RuleFor(a => a.MeatRate, "50%")
                .RuleFor(a => a.EatingType, EatingType.both)
                .Generate();
            var expected = 42 * 0.5 * 10 + 42 * 0.5 * 7;

            // Act
            var actual = _service.CalculateAmount(animal, foodPrices);

            // Assert
            actual.Equals(expected);
        }

        [Fact]
        [Trait("Category", "Service")]
        [Trait("Category", nameof(Implementations.FoodPriceService))]
        public async Task Should_CalculateAmount_As_0_When_Exception_Is_Thrown()
        {
            // Arrange
            var foodPrices = new Faker<FoodPrices>().Generate();
            var animal = new Faker<ZooAnimal>()
                .RuleFor(a => a.Name, f => f.Name.FirstName())
                .RuleFor(a => a.Weight, f => f.Random.Decimal(0.1m, 200))
                .RuleFor(a => a.FoodWeightRate, f => f.Random.Decimal().OrNull(f, 1f))
                .Generate();

            // Act
            var actual = _service.CalculateAmount(animal, foodPrices);

            // Assert
            actual.Equals(default);
            _loggerMock.Verify(x => x.Information("{0} to calculate food cost amount for animal with Name '{1}'", "Trying", animal.Name), Times.Once);
        }

        [Fact]
        [Trait("Category", "Service")]
        [Trait("Category", nameof(Implementations.FoodPriceService))]
        public void Should_CalculateAmount_As_0_When_Input_Animal_Null()
        {
            // Arrange
            var foodPrices = new Faker<FoodPrices>().Generate();

            // Act
            var actual = Assert.Throws<ArgumentNullException>(() => _service.CalculateAmount(null, foodPrices));

            // Assert
            Assert.Equal("animal", actual.ParamName);
        }

        [Fact]
        [Trait("Category", "Service")]
        [Trait("Category", nameof(Implementations.FoodPriceService))]
        public void Should_CalculateAmount_As_0_When_Input_FoodPrices_Null()
        {
            // Arrange
            var animal = new Faker<ZooAnimal>().Generate();

            // Act
            var actual = Assert.Throws<ArgumentNullException>(() => _service.CalculateAmount(animal, null));

            // Assert
            Assert.Equal("prices", actual.ParamName);
        }
    }
}
