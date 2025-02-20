using Microsoft.Extensions.Logging;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventEmitter.Abstractions;

public abstract class BaseNetworkEventEmitter<TSerialized> : INetworkEventEmitter
{
    private readonly IEventSerializerProvider<TSerialized> _serializerProvider;
    private readonly ILogger _logger;

    protected BaseNetworkEventEmitter(IEventSerializerProvider<TSerialized> serializerProvider,
        ILogger logger)
    {
        _serializerProvider = serializerProvider;
        _logger = logger;
    }

    public Task EmitEvent<TEvent>(TEvent @event) where TEvent : class, IBaseEvent
    {
        TSerialized payload;
        IEventSerializer<TSerialized, TEvent> serializer;
        try
        {
            serializer = _serializerProvider.GetSerializer<TEvent>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Failed to get serializer for {typeof(TEvent)}");
            throw;
        }

        try
        {
            payload = serializer.Serialize(@event);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to serialize payload");
            throw;
        }

        try
        {
            return BroadcastEvent(payload);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Event emission failed");
            throw;
        }
    }

    protected abstract Task BroadcastEvent(TSerialized serialized);
}