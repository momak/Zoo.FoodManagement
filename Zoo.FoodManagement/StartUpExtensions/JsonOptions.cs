using System.Text.Json;
using System.Text.Json.Serialization;

namespace Zoo.FoodManagement.StartUpExtensions;

#pragma warning disable CS1591
public static class JsonOptions
{
    public static IMvcBuilder AddJsonOptions(this IMvcBuilder builder)
    {
        return builder.AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
    }
}