using System;
using TranCons.Shared.NetworkEvents;

namespace TranCons.EventProcessors.Implementation
{
    public class TypeSpeedEvent : IBaseEvent
    {
        public DateTimeOffset DateTime { get; } = DateTimeOffset.Now;
        public double TypeSpeed { get; }

        internal TypeSpeedEvent(double typeSpeed)
        {
            TypeSpeed = typeSpeed;
        }
    }
}
