using TranCons.Shared.NetworkEvents;

namespace TranCons.EventListener.Abstractions;

public interface IEventDeserializer<in TSerialized, TEvent>
{
    bool TryDeserialize(TSerialized serialized, out TEvent result);
    TEvent Deserialize(TSerialized serialized);
}

public interface IEventDeserializer<in TSerialized>
{
    bool TryDeserialize(TSerialized serialized, out IBaseEvent result);
    IBaseEvent Deserialize(TSerialized serialized);
}