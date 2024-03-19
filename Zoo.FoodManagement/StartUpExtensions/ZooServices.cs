using Zoo.Services.Abstractions;
using Zoo.Services.Implementations;

namespace Zoo.FoodManagement.StartUpExtensions;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public static class ZooServices
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services
            .AddTransient<IFile, FileWrapper>()
            .AddTransient<IFoodPriceService, FoodPriceService>()
            .AddTransient<IZooPriceService, ZooPriceService>()
            .AddTransient<TxtFileLoaderService>()
            .AddTransient<CsvFileLoaderService>()
            .AddTransient<XmlFileLoaderService>();

        services.AddTransient<IFileLoaderService, TxtFileLoaderService>();
        services.AddTransient<IFileLoaderService, XmlFileLoaderService>();
        services.AddTransient<IFileLoaderService, CsvFileLoaderService>();
        services.AddTransient<FileLoaderFactory>();

        //services
        //    .AddScoped<Func<string, IFileLoaderService>>(fileLoaderProvider => key =>
        //    {
        //        if (key.EndsWith(".txt"))
        //        {
        //            return fileLoaderProvider.GetService<TxtFileLoaderService>()!;
        //        }
        //        if (key.EndsWith(".xml"))
        //        {
        //            return fileLoaderProvider.GetService<XmlFileLoaderService>()!;
        //        }
        //        return (key.EndsWith(".csv") 
        //            ? fileLoaderProvider.GetService<CsvFileLoaderService>()
        //            : null)!;
        //    });

        return services;
    }
}