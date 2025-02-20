using System;
using TranCons.EventListener.Abstractions;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventProcessors.Abstractions;

public abstract class BaseEventProcessor<TInputEvent, TOutputEvent> : IEventProcessor
    where TInputEvent : class, IBaseEvent
    where TOutputEvent : class, IBaseEvent
{
    private readonly INetworkEventListener _eventListener;

    public string Name { get; }

    protected BaseEventProcessor(INetworkEventListener eventListener, string name)
    {
        _eventListener = eventListener;
        Name = name;
    }

    protected abstract IObservable<TOutputEvent> Process(IObservable<TInputEvent> source);

    public IObservable<IBaseEvent> ObserveProcessedEvents()
    {
        return Process(_eventListener.ObserveEventsOfType<TInputEvent>());
    }
}