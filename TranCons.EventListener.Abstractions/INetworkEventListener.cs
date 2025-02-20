using TranCons.Shared.NetworkEvents;

namespace TranCons.EventListener.Abstractions;

public interface INetworkEventListener
{
    IObservable<TEvent> ObserveEventsOfType<TEvent>();
    IObservable<IBaseEvent> ObserveAllEvents();
}