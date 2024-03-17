using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using System.Reflection;
using Zoo.FoodManagement.StartUpExtensions;

try
{
    var builder = WebApplication.CreateBuilder(args);

    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .CreateBootstrapLogger();

    builder.WebHost
        .UseContentRoot(Directory.GetCurrentDirectory())
        .ConfigureAppConfiguration((context, config) =>
        {
            SetConfigurations(config, context.HostingEnvironment.EnvironmentName, args);

        })
        .ConfigureLogging((context, logging) =>
        {
            logging
                .ClearProviders()
                .AddSerilog()
                .AddConsole();
        });

    builder.Host.ConfigureSerilog();

    builder.Services
        .AddConfigurationServices(builder.Configuration);
    builder.Services
        .AddResponseCaching()
        .AddServices();

    builder.Services
        .AddControllers()
        .AddJsonOptions();

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Zoo Management API",
            Version = "v1",
            Description = "Technical assignment"
        });

        c.UseInlineDefinitionsForEnums();
        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();


}
catch (Exception ex)
{
    // Log.Logger will likely be internal type "Serilog.Core.Pipeline.SilentLogger".
    if (Log.Logger == null || Log.Logger.GetType().Name == "SilentLogger")
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
    }

    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

static IConfigurationBuilder SetConfigurations(IConfigurationBuilder configuration, string environment, string[] args)
{
    configuration
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .AddJsonFile($"appsettings.{environment}.json", optional: true)
        .AddEnvironmentVariables();

    if (args != null)
    {
        configuration.AddCommandLine(args);
    }
    return configuration;
}