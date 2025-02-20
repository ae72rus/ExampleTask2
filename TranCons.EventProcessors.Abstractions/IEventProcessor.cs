using System;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventProcessors.Abstractions
{
    public interface IEventProcessor
    {
        string Name { get; }
        IObservable<IBaseEvent> ObserveProcessedEvents();
    }
}
