using Serilog;

namespace Zoo.Services.Implementations;

public abstract class BaseService
{
    protected readonly ILogger logger;

    protected BaseService(ILogger logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
}