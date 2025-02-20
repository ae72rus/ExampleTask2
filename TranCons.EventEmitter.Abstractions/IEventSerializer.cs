using TranCons.Shared.NetworkEvents;

namespace TranCons.EventEmitter.Abstractions;

public interface IEventSerializer<out TSerialized, in TEvent>
where TEvent: class,  IBaseEvent
{
    TSerialized Serialize(TEvent @event);
}