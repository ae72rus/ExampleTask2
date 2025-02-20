using System;
using System.Reactive.Linq;
using TranCons.EventListener.Abstractions;
using TranCons.EventProcessors.Abstractions;
using TranCons.Shared.DTO;

namespace TranCons.EventProcessors.Implementation;

public class InstantTypeSpeedMeter : BaseEventProcessor<InputEvent, TypeSpeedEvent>
{
    private DateTimeOffset _lastEventTime = DateTimeOffset.MinValue;
    private readonly IObservable<long> _interval = Observable.Interval(TimeSpan.FromMilliseconds(200));
    public InstantTypeSpeedMeter(INetworkEventListener eventListener)
        : base(eventListener, "Type Speed")
    {
    }

    protected override IObservable<TypeSpeedEvent> Process(IObservable<InputEvent> source)
    {
        var timeBased = _interval.Select(x => new TypeSpeedEvent(0));
        var eventBased = source.TimeInterval()
            .Select(x =>
            {
                _lastEventTime = DateTimeOffset.Now;
                var millisecondsSincePrevEvent = x.Interval.TotalMilliseconds;
                var speed = millisecondsSincePrevEvent == 0
                    ? 0
                    : 60 * 1000 / millisecondsSincePrevEvent;
                return new TypeSpeedEvent(speed);
            });

        return eventBased.CombineLatest(timeBased)
            .Select((tuple, _) =>
                (DateTimeOffset.Now - _lastEventTime).TotalMilliseconds > 200
                    ? tuple.Second
                    : tuple.First);
    }
}