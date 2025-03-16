using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlazorColorPicker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddColorPicker(this IServiceCollection services)
    {
        return services.AddColorPicker(ServiceLifetime.Scoped);
    }

    public static IServiceCollection AddColorPicker(this IServiceCollection services, ServiceLifetime serviceLifetime)
    {
        services.TryAdd(new ServiceDescriptor(typeof(IColorPickerService), typeof(ColorPickerService), serviceLifetime));
        return services;
    }
}
