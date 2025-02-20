using System;
using TranCons.EventListener.Abstractions;
using TranCons.EventProcessors.Abstractions;
using TranCons.Shared.DTO;

namespace TranCons.EventProcessors.Implementation;

public class PressedKeyViewer : BaseEventProcessor<InputEvent, InputEvent>
{
    public PressedKeyViewer(INetworkEventListener eventListener)
        : base(eventListener, "Pressed key")
    {
    }

    protected override IObservable<InputEvent> Process(IObservable<InputEvent> source)
        => source;
}