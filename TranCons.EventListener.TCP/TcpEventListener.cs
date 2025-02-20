using Microsoft.Extensions.Logging;
using TranCons.EventListener.Abstractions;

namespace TranCons.EventListener.TCP;

internal class TcpEventListener : BaseEventListener<byte[]>
{
    public TcpEventListener(IEventDeserializerProvider<byte[]> deserializerProvider,
        ILogger logger,
        ITcpClient client)
        : base(deserializerProvider, logger)
    {
        client.ServerMessageHandler = OnEvent;
    }
}