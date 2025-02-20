using TranCons.EventListener.Abstractions;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventListener.TCP;

internal class TcpDeserializerProvider : IEventDeserializerProvider<byte[]>
{
    public IEventDeserializer<byte[], TEvent> GetDeserializer<TEvent>(byte[] serialized)
        where TEvent : class, IBaseEvent
    {
        return new TcpEventDeserializer<TEvent>();
    }

    public IEventDeserializer<byte[]> GetDeserializer(byte[] serialized)
    {
        return new TcpEventDeserializer();
    }
}