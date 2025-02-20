using Microsoft.Extensions.Logging;
using TranCons.EventEmitter.Abstractions;

namespace TranCons.EventEmitter.TCP;

internal class TcpEventEmitter : BaseNetworkEventEmitter<byte[]>
{
    private readonly ITcpServer _server;

    public TcpEventEmitter(IEventSerializerProvider<byte[]> serializerProvider, 
        ILogger logger,
        ITcpServer server)
        : base(serializerProvider, logger)
    {
        _server = server;
    }

    protected override Task BroadcastEvent(byte[] serialized)
    {
        return _server.SendAsync(serialized);
    }
}