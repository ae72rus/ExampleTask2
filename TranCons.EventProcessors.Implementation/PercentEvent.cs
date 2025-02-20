using System;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventProcessors.Implementation;

public class PercentEvent : IBaseEvent
{
    public DateTimeOffset DateTime { get; } = DateTimeOffset.Now;
    public double Percent { get; }//from 0 to 1

    internal PercentEvent(double percent)
    {
        //if(percent is < 0 or > 1)
        //    throw new ArgumentOutOfRangeException(nameof(percent));

        Percent = percent;
    }
}