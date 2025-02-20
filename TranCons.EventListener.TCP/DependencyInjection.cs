using Microsoft.Extensions.DependencyInjection;
using TranCons.EventListener.Abstractions;

namespace TranCons.EventListener.TCP;

public static class DependencyInjection
{
    public static IServiceCollection AddTcpEventListener(this IServiceCollection services,
        Action<TcpClientConfiguration> setupAction)
    {
        var config = new TcpClientConfiguration();
        setupAction.Invoke(config);
        return services
            .AddSingleton(config)
            .AddSingleton<IEventDeserializerProvider<byte[]>, TcpDeserializerProvider>()
            .AddSingleton<INetworkEventListener, TcpEventListener>()
            .AddSingleton<ITcpClient, TcpClient>();
    }
}