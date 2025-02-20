using TranCons.EventEmitter.Abstractions;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventEmitter.TCP.Tests.TestImplementations;

internal class TestSerializerProvider : IEventSerializerProvider<byte[]>
{
    public IEventSerializer<byte[], TEvent> GetSerializer<TEvent>()
        where TEvent : class, IBaseEvent
    {
        return new TestSerializer<TEvent>();
    }
}