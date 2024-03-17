using Serilog;

namespace Zoo.FoodManagement.StartUpExtensions;

#pragma warning disable CS1591
public static class Serilog
{
    public static IHostBuilder ConfigureSerilog(this ConfigureHostBuilder builder)
    {
        builder.UseSerilog((hostingContext, services, loggerConfiguration) =>
        {
            loggerConfiguration
                .ReadFrom.Configuration(hostingContext.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext();

        });

        return builder;
    }
}