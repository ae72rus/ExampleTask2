using TranCons.Shared.NetworkEvents;

namespace TranCons.EventEmitter.Abstractions;

public interface IEventSerializerProvider<out TSerialized>
{
    IEventSerializer<TSerialized, TEvent> GetSerializer<TEvent>()
        where TEvent : class, IBaseEvent;
}