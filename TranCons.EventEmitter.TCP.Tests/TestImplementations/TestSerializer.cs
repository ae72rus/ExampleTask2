using System.Text;
using TranCons.EventEmitter.Abstractions;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventEmitter.TCP.Tests.TestImplementations;

internal class TestSerializer<TEvent> : IEventSerializer<byte[], TEvent>
    where TEvent : class, IBaseEvent
{
    public byte[] Serialize(TEvent @event)
    {
        return Encoding.Default.GetBytes(@event.ToString()!);
    }
}