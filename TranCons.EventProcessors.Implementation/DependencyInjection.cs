using Microsoft.Extensions.DependencyInjection;
using TranCons.EventProcessors.Abstractions;

namespace TranCons.EventProcessors.Implementation;

public static class DependencyInjection
{
    public static IServiceCollection AddBasicEventProcessors(this IServiceCollection services)
    {
        return services
            .AddScoped<IEventProcessor, AvgTypeSpeedMeter>()
            .AddScoped<IEventProcessor, InstantTypeSpeedMeter>()
            .AddScoped<IEventProcessor, MostPopularKeyCalculator>()
            .AddScoped<IEventProcessor, TypeUniformityMeter>()
            .AddScoped<IEventProcessor, PressedKeyViewer>();
    }
}