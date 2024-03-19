using FluentAssertions;
using Moq;
using Serilog;
using Zoo.Models;
using Zoo.Models.Abstractions;
using Zoo.Services.Abstractions;

namespace Zoo.Services.Test.FileLoaderService;

public class CsvFileLoaderService
{
    private readonly Mock<IFile> _fileWrapperMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly CancellationToken _token;

    private readonly Implementations.CsvFileLoaderService _service;

    public CsvFileLoaderService()
    {
        _fileWrapperMock = new Mock<IFile>();
        _loggerMock = new Mock<ILogger>();
        _token = CancellationToken.None;
        _service = new Implementations.CsvFileLoaderService(_fileWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(Implementations.CsvFileLoaderService))]
    public async Task Should_LoadContent_From_File()
    {
        // Arrange
        _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);
        _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".csv");

        // Act
        var actual = await _service.LoadDataContent("MockResources/test.csv", _token);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<IZoo>();
        actual.Should().BeOfType<AnimalTypeConfig>().Subject.Configurations.Count.Should().Be(6);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(Implementations.CsvFileLoaderService))]
    public async Task Should_LoadContent_From_EmptyFile_With_Default_Result()
    {
        // Arrange
        _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);
        _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".csv");

        // Act
        var actual = await _service.LoadDataContent("MockResources/test_Empty.csv", _token);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<IZoo>();
        actual.Should().BeOfType<AnimalTypeConfig>().Subject.Configurations.Count.Should().Be(0);

        _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Once);
        _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Once);

    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(Implementations.CsvFileLoaderService))]
    public async Task Should_LoadContent_From_IncorrectFile_With_Default_Result()
    {
        // Arrange
        _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);
        _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".csv");

        // Act
        var actual = await _service.LoadDataContent("MockResources/test_Incorrect.csv", _token);

        // Assert
        actual.Should().BeNull();

        _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Once);
        _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Once);

    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(Implementations.CsvFileLoaderService))]
    public async Task Should_LoadContent_From_IncorrectFileFormat_With_Default_Result()
    {
        // Arrange
        _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);
        _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".xml");

        // Act
        var actual = await _service.LoadDataContent("MockResources/test_Incorrect.xml", _token);

        // Assert
        actual.Should().BeNull();

        _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Once);
        _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Once);

    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(Implementations.CsvFileLoaderService))]
    public async Task Should_LoadContent_From_On_Empty_Path()
    {
        // Arrange
        _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true).Verifiable();
        _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".csv").Verifiable();

        // Act
        var actual = await _service.LoadDataContent(string.Empty, _token);

        // Assert
        actual.Should().BeNull();
        _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Never);
        _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Never);

    }
}