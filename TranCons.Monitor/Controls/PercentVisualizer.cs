using TranCons.EventProcessors.Implementation;

namespace TranCons.Monitor.Controls;

public class PercentVisualizer : BaseChartVisualizer<PercentEvent>
{
    public PercentVisualizer()
    {
        UseAutoScale = false;
    }
    protected override double GetEventValue(PercentEvent @event)
        => @event.Percent;
}