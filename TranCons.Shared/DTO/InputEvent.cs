using TranCons.Shared.NetworkEvents;

namespace TranCons.Shared.DTO;

[Serializable]
public class InputEvent: IBaseEvent
{
    public DateTimeOffset DateTime { get; }
    public InputKey Key { get; }
    public InputKey SystemKey { get; }

    internal InputEvent()
    {
        //used for deserialization
    }

    public InputEvent(InputKey key, InputKey systemKey)
    {
        DateTime = DateTimeOffset.Now;
        Key = key;
        SystemKey = systemKey;
    }
}