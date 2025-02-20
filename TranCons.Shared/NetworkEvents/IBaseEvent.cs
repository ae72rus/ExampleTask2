namespace TranCons.Shared.NetworkEvents;

public interface IBaseEvent
{
    DateTimeOffset DateTime { get; }
}