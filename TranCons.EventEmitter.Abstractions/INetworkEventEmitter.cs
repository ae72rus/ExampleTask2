using TranCons.Shared.NetworkEvents;

namespace TranCons.EventEmitter.Abstractions;

public interface INetworkEventEmitter
{
    Task EmitEvent<TEvent>(TEvent @event)
        where TEvent : class, IBaseEvent;
}