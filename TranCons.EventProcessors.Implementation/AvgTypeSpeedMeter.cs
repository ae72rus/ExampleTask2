using System;
using System.Reactive.Linq;
using TranCons.EventListener.Abstractions;
using TranCons.EventProcessors.Abstractions;
using TranCons.Shared.DTO;

namespace TranCons.EventProcessors.Implementation;

public class AvgTypeSpeedMeter : BaseEventProcessor<InputEvent, TypeSpeedEvent>
{
    private readonly DateTimeOffset _startTime = DateTimeOffset.Now;
    private ulong _counter;

    private readonly IObservable<long> _interval = Observable.Interval(TimeSpan.FromMilliseconds(200));

    public AvgTypeSpeedMeter(INetworkEventListener eventListener)
        : base(eventListener, "Avg. Type speed")
    {
    }

    protected override IObservable<TypeSpeedEvent> Process(IObservable<InputEvent> source)
    {
        var timeBased = _interval.Select(x => new TypeSpeedEvent(GetTypeSpeed()));
        var eventBased = source.Select(x =>
          {
              ++_counter;
              return new TypeSpeedEvent(GetTypeSpeed());
          });

        return eventBased.CombineLatest(timeBased)
            .Select(tuple => tuple.First.DateTime > tuple.Second.DateTime
                ? tuple.First
                : tuple.Second);
    }

    private double GetTypeSpeed()
    {
        var secondsSinceStart = (DateTimeOffset.Now - _startTime).TotalSeconds;
        var speed = secondsSinceStart == 0
            ? 0
            : _counter / secondsSinceStart * 60;
        return speed;
    }
}