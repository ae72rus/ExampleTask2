using Microsoft.Extensions.DependencyInjection;
using TranCons.EventEmitter.Abstractions;

namespace TranCons.EventEmitter.TCP;

public static class DependencyInjection
{
    public static IServiceCollection AddTcpEventEmitter(this IServiceCollection services, Action<TcpServerConfiguration> setupAction)
    {
        var config = new TcpServerConfiguration();
        setupAction.Invoke(config);
        return services
            .AddSingleton(config)
            .AddSingleton<ITcpServer, TcpServer>()//server runs in instantiation
            .AddSingleton<INetworkEventEmitter, TcpEventEmitter>()
            .AddSingleton<IEventSerializerProvider<byte[]>, BinarySerializerProvider>();
    }
}