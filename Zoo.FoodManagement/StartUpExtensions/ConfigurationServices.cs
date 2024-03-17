using Zoo.Common.Infrastructure;

namespace Zoo.FoodManagement.StartUpExtensions
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class ConfigurationServices
    {
        public static IServiceCollection AddConfigurationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ExternalSettings>(configuration.GetSection(nameof(ExternalSettings)));
            services.AddSingleton<IConfiguration>(configuration);

            return services;
        }
    }
}



