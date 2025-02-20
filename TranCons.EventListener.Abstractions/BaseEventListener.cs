using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Extensions.Logging;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventListener.Abstractions;

public abstract class BaseEventListener<TSerialized> : INetworkEventListener
{
    private readonly IEventDeserializerProvider<TSerialized> _deserializerProvider;
    private readonly ILogger _logger;
    private readonly Subject<IBaseEvent> _masterSubject = new();

    protected BaseEventListener(IEventDeserializerProvider<TSerialized> deserializerProvider,
        ILogger logger)
    {
        _deserializerProvider = deserializerProvider;
        _logger = logger;
    }

    public IObservable<TEvent> ObserveEventsOfType<TEvent>()
    {
        return _masterSubject.OfType<TEvent>();
    }

    public IObservable<IBaseEvent> ObserveAllEvents()
    {
        return _masterSubject;
    }

    protected void OnEvent(TSerialized serialized)
    {
        IEventDeserializer<TSerialized>? deserializer = null;
        IBaseEvent @event;

        try
        {
            deserializer = _deserializerProvider.GetDeserializer(serialized);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to get deserializer");
        }

        try
        {
            @event = deserializer!.Deserialize(serialized);
        }
        catch (Exception e)
        {
            _logger.LogCritical(e, "Failed to deserialize event");
            throw;
        }

        _masterSubject.OnNext(@event);
    }
}