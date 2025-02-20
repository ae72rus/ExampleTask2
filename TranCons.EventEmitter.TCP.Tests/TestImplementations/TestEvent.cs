using TranCons.Shared.NetworkEvents;

namespace TranCons.EventEmitter.TCP.Tests.TestImplementations;

internal class TestEvent : IBaseEvent
{
    public DateTimeOffset DateTime { get; } = DateTimeOffset.UtcNow;
    public Guid Guid { get; } = Guid.NewGuid();
}