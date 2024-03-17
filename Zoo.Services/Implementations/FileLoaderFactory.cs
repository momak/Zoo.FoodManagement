using Zoo.Services.Abstractions;

namespace Zoo.Services.Implementations;

public class FileLoaderFactory 
{
    private readonly IEnumerable<IFileLoaderService> _fileLoaderServices;

    public FileLoaderFactory(IEnumerable<IFileLoaderService> fileLoaderServices)
    {
        _fileLoaderServices = fileLoaderServices;
    }

    public IFileLoaderService GetFileLoaderService(string key)
    {
        return _fileLoaderServices.FirstOrDefault(e => key.EndsWith(e.Mode()))
               ?? throw new NotSupportedException();
    }
}