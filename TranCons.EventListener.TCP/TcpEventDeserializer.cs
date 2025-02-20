using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using TranCons.EventListener.Abstractions;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventListener.TCP;

internal class TcpEventDeserializer<TEvent> : TcpEventDeserializer, IEventDeserializer<byte[], TEvent>
    where TEvent : class, IBaseEvent
{
    public bool TryDeserialize(byte[] serialized, out TEvent result)
    {
        var candidate = DeserializeInternal(serialized) as TEvent;
        result = candidate;

        return candidate != null;
    }

    public override TEvent Deserialize(byte[] serialized)
    {
        var result = base.Deserialize(serialized) as TEvent;
        return result ?? throw new SerializationException($"Unable to deserialize byte array. Array: {Convert.ToBase64String(serialized)}");
    }
}

internal class TcpEventDeserializer : IEventDeserializer<byte[]>
{
    public bool TryDeserialize(byte[] serialized, out IBaseEvent result)
    {
        var candidate = DeserializeInternal(serialized);
        result = candidate;

        return candidate != null;
    }

    public virtual IBaseEvent Deserialize(byte[] serialized)
    {
        var result = DeserializeInternal(serialized);
        return result ??
               throw new SerializationException(
                   $"Unable to deserialize byte array. Array: {Convert.ToBase64String(serialized)}");
    }

    protected IBaseEvent? DeserializeInternal(byte[] bytes)
    {
        var formatter = new BinaryFormatter();
        using var memStream = new MemoryStream(bytes);
        var result = formatter.Deserialize(memStream) as IBaseEvent;
        return result;
    }
}