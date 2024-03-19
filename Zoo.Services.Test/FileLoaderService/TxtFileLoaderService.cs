using FluentAssertions;
using Moq;
using Serilog;
using Zoo.Models;
using Zoo.Models.Abstractions;
using Zoo.Services.Abstractions;

namespace Zoo.Services.Test.FileLoaderService
{
    public class TxtFileLoaderService
    {
        private readonly Mock<IFile> _fileWrapperMock;
        private readonly Mock<ILogger> _loggerMock;
        private readonly CancellationToken _token;

        private readonly Implementations.TxtFileLoaderService _service;

        public TxtFileLoaderService()
        {
            _fileWrapperMock = new Mock<IFile>();
            _loggerMock = new Mock<ILogger>();
            _token = CancellationToken.None;
            _service = new Implementations.TxtFileLoaderService(_fileWrapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        [Trait("Category", "Service")]
        [Trait("Category", nameof(Implementations.TxtFileLoaderService))]
        public async Task Should_LoadContent_From_File()
        {
            // Arrange
            _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(true);
            _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
                .Returns(".txt");
            _fileWrapperMock.Setup(x => x.ReadLinesAsync(It.IsAny<string>(), _token))
                .Returns(FileContentMock);

            // Act
            var actual = await _service.LoadDataContent("path/to/file.txt", _token);

            // Assert
            actual.Should().NotBeNull();
            actual.Should().BeAssignableTo<ZooBase>();
            actual.Should().BeOfType<FoodPrices>();
        }

        [Fact]
        [Trait("Category", "Service")]
        [Trait("Category", nameof(Implementations.TxtFileLoaderService))]
        public async Task Should_LoadContent_From_File_Empty()
        {
            // Arrange
            _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(true).Verifiable();
            _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
                .Returns(".txt").Verifiable();
            _fileWrapperMock.Setup(x => x.ReadLinesAsync(It.IsAny<string>(), _token))
                .Returns(FileContentMock).Verifiable();

            // Act
            var actual = await _service.LoadDataContent(string.Empty, _token);

            // Assert
            actual.Should().BeNull();
            _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Never);
            _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Never);
            _fileWrapperMock.Verify(x => x.ReadLinesAsync(It.IsAny<string>(), _token), Times.Never);

        }

        [Fact]
        [Trait("Category", "Service")]
        [Trait("Category", nameof(Implementations.TxtFileLoaderService))]
        public async Task Should_LoadContent_From_File_Empty_When_FileNotFound()
        {
            // Arrange
            _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(false).Verifiable();
            _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
                .Returns(".txt").Verifiable();
            _fileWrapperMock.Setup(x => x.ReadLinesAsync(It.IsAny<string>(), _token))
                .Returns(FileContentMock).Verifiable();

            // Act
            var actual = await _service.LoadDataContent("file/does/not/exists.txt", _token);

            // Assert
            actual.Should().BeNull();
            _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Once);
            _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Never);
            _fileWrapperMock.Verify(x => x.ReadLinesAsync(It.IsAny<string>(), _token), Times.Never);

        }

        [Fact]
        [Trait("Category", "Service")]
        [Trait("Category", nameof(Implementations.TxtFileLoaderService))]
        public async Task Should_LoadContent_From_File_Empty_When_Extension_Wrong()
        {
            // Arrange
            _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(true).Verifiable();
            _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
                .Returns(".xml").Verifiable();
            _fileWrapperMock.Setup(x => x.ReadLinesAsync(It.IsAny<string>(), _token))
                .Returns(FileContentMock).Verifiable();

            // Act
            var actual = await _service.LoadDataContent("file/does/not/exists.xml", _token);

            // Assert
            actual.Should().BeNull();
            _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Once);
            _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Once);
            _fileWrapperMock.Verify(x => x.ReadLinesAsync(It.IsAny<string>(), _token), Times.Never);

        }

        [Fact]
        [Trait("Category", "Service")]
        [Trait("Category", nameof(Implementations.TxtFileLoaderService))]
        public async Task Should_LoadContent_Throw_Exception_On_IncorrectData()
        {
            // Arrange
            _fileWrapperMock.Setup(x => x.Exists(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();
            _fileWrapperMock.Setup(x => x.GetExtension(It.IsAny<string>()))
                .Returns(".txt").Verifiable();
            _fileWrapperMock.Setup(x => x.ReadLinesAsync(It.IsAny<string>(), _token))
                .Returns(FileIncorrectContentMock)
                .Verifiable();

            // Act
            var actual = await _service.LoadDataContent("file/path.txt", _token);

            // Assert
            actual.Should().BeNull();
            _fileWrapperMock.Verify(x => x.Exists(It.IsAny<string>()), Times.Once);
            _fileWrapperMock.Verify(x => x.GetExtension(It.IsAny<string>()), Times.Once);
            _fileWrapperMock.Verify(x => x.ReadLinesAsync(It.IsAny<string>(), _token), Times.AtLeastOnce);

        }

        private async IAsyncEnumerable<string> FileContentMock()
        {
            yield return "Meat=10.32";
            yield return "Fruit=11.43";

            await Task.CompletedTask; // to make the compiler warning go away
        }

        private async IAsyncEnumerable<string> FileIncorrectContentMock()
        {
            yield return "M=10.32";
            yield return "Fruit=11.43";

            await Task.CompletedTask; // to make the compiler warning go away
        }
    }
}
