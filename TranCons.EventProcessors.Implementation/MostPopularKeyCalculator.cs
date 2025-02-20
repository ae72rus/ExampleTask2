using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using TranCons.EventListener.Abstractions;
using TranCons.EventProcessors.Abstractions;
using TranCons.Shared.DTO;

namespace TranCons.EventProcessors.Implementation;

public class MostPopularKeyCalculator : BaseEventProcessor<InputEvent, InputEvent>
{
    private readonly Dictionary<InputKey, ulong> _counters = new();
    public MostPopularKeyCalculator(INetworkEventListener eventListener)
        : base(eventListener, "Avg. Key")
    {
    }

    protected override IObservable<InputEvent> Process(IObservable<InputEvent> source)
    {
        return source
            .Select(e =>
            {
                _counters[e.Key] = _counters.ContainsKey(e.Key)
                    ? _counters[e.Key] + 1
                    : 1;

                var popularKey = GetMostPopularKey();
                return new InputEvent(popularKey, InputKey.None);
            });
    }

    private InputKey GetMostPopularKey()
    {
        var maxPair = _counters.MaxBy(x => x.Value);
        return maxPair.Key;
    }
}