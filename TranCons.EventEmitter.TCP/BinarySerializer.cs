using System.Runtime.Serialization.Formatters.Binary;
using TranCons.EventEmitter.Abstractions;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventEmitter.TCP;

internal class BinarySerializer<TEvent> : IEventSerializer<byte[], TEvent>
    where TEvent : class, IBaseEvent
{
    public byte[] Serialize(TEvent @event)
    {
        var formatter = new BinaryFormatter();
        using var memStream = new MemoryStream();
        formatter.Serialize(memStream, @event);
        memStream.Flush();
        return memStream.ToArray();
    }
}