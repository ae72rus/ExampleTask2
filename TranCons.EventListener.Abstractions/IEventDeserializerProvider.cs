using TranCons.Shared.NetworkEvents;

namespace TranCons.EventListener.Abstractions;

public interface IEventDeserializerProvider<in TSerialized>
{
    IEventDeserializer<TSerialized, TEvent> GetDeserializer<TEvent>(TSerialized serialized)
        where TEvent : class, IBaseEvent;
    IEventDeserializer<TSerialized> GetDeserializer(TSerialized serialized);
}