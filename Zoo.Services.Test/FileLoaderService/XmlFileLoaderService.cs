using FluentAssertions;
using Moq;
using Serilog;
using Zoo.Models.Abstractions;
using Zoo.Services.Abstractions;

namespace Zoo.Services.Test.FileLoaderService;

public class XmlFileLoaderService
{
    private readonly Mock<IFile> _fileWrapperMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly CancellationToken _token;

    private readonly Implementations.XmlFileLoaderService _service;

    public XmlFileLoaderService()
    {
        _fileWrapperMock = new Mock<IFile>();
        _loggerMock = new Mock<ILogger>();
        _token = CancellationToken.None;
        _service = new Implementations.XmlFileLoaderService(_fileWrapperMock.Object, _loggerMock.Object);
    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(Implementations.XmlFileLoaderService))]
    public async Task Should_LoadContent_From_File()
    {
        // Arrange
        _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);
        _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".xml");

        // Act
        var actual = await _service.LoadDataContent("MockResources/test.xml", _token);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<IZoo>();
        actual.Should().BeOfType<Models.Zoo>().Subject.Animals.Count.Should().Be(8);
    }

    [Theory]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(Implementations.XmlFileLoaderService))]
    [InlineData("MockResources/test_Empty.xml")]
    [InlineData("MockResources/test_Incorrect.xml")]
    public async Task Should_LoadContent_From_File_With_Default_Result(string filePath)
    {
        // Arrange
        _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true);
        _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".xml");

        // Act
        var actual = await _service.LoadDataContent(filePath, _token);

        // Assert
        actual.Should().NotBeNull();
        actual.Should().BeAssignableTo<IZoo>();
        actual.Should().BeOfType<Models.Zoo>().Subject.Animals.Count.Should().Be(0);
        
        _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Once);
        _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Once);

    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(Implementations.XmlFileLoaderService))]
    public async Task Should_LoadContent_From_File_Empty_When_Extension_Wrong()
    {
        // Arrange
        _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true).Verifiable();
        _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".txt").Verifiable();
       
        // Act
        var actual = await _service.LoadDataContent("file/does/not/exists.txt", _token);

        // Assert
        actual.Should().BeNull();
        _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Once);
        _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Once);

    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(Implementations.XmlFileLoaderService))]
    public async Task Should_LoadContent_From_File_Corrupted()
    {
        // Arrange
        _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true).Verifiable();
        _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".xml").Verifiable();

        // Act
        var actual = await _service.LoadDataContent("file/does/not/exists.xml", _token);

        // Assert
        actual.Should().BeNull();
        _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Once);
        _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Once);

    }

    [Fact]
    [Trait("Category", "Service")]
    [Trait("Category", nameof(Implementations.XmlFileLoaderService))]
    public async Task Should_LoadContent_From_On_Empty_Path()
    {
        // Arrange
        _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
            .Returns(true).Verifiable();
        _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
            .Returns(".xml").Verifiable();

        // Act
        var actual = await _service.LoadDataContent(string.Empty, _token);

        // Assert
        actual.Should().BeNull();
        _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Never);
        _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Never);

    }
}