using TranCons.EventEmitter.Abstractions;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventEmitter.TCP;

internal class BinarySerializerProvider : IEventSerializerProvider<byte[]>
{
    public IEventSerializer<byte[], TEvent> GetSerializer<TEvent>() where TEvent : class, IBaseEvent 
        => new BinarySerializer<TEvent>();
}