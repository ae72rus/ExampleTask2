using System;
using System.Reactive.Linq;
using TranCons.EventListener.Abstractions;
using TranCons.EventProcessors.Abstractions;
using TranCons.Shared.DTO;

namespace TranCons.EventProcessors.Implementation;

public class TypeUniformityMeter : BaseEventProcessor<InputEvent, PercentEvent>
{
    private double _avgIntervalMs;
    private double _intervals;
    private int _counter;
    public TypeUniformityMeter(INetworkEventListener eventListener)
        : base(eventListener, "Type Uniformity")
    {
    }

    protected override IObservable<PercentEvent> Process(IObservable<InputEvent> source)
    {
        return source
            .TimeInterval()
            .Select(e =>
            {
                _intervals += e.Interval.TotalMilliseconds;
                _avgIntervalMs = _intervals / ++_counter;
                
                var diff = Math.Abs(e.Interval.TotalMilliseconds - _avgIntervalMs);
                var ratio = 1 - diff / (e.Interval.TotalMilliseconds + _avgIntervalMs);

                return new PercentEvent(ratio);
            });
    }
}