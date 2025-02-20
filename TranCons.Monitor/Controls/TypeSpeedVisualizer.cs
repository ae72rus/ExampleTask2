using TranCons.EventProcessors.Implementation;

namespace TranCons.Monitor.Controls;

public class TypeSpeedVisualizer : BaseChartVisualizer<TypeSpeedEvent>
{
    protected override double GetEventValue(TypeSpeedEvent @event)
        => @event.TypeSpeed;
}