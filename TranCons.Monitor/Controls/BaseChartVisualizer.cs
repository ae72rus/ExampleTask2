using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TranCons.Shared.NetworkEvents;

namespace TranCons.Monitor.Controls;

public abstract class BaseChartVisualizer<TValueEvent> : BaseGraphVisualizer
    where TValueEvent : class, IBaseEvent
{
    public static DependencyProperty StrokeProperty = DependencyProperty.Register(nameof(Stroke),
        typeof(Brush),
        typeof(BaseChartVisualizer<TValueEvent>),
        new PropertyMetadata(new SolidColorBrush(Colors.Black)));

    private Polyline? _line;
    private Label? _valueLabel;
    private double _min = 0;
    private double _max = 1;
    private readonly List<TValueEvent> _displayedEvents = new();

    public Brush Stroke
    {
        get => (Brush)GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    protected bool UseAutoScale { get; set; } = true;

    protected override void DrawInternal()
    {
        var events = Events
            .Cast<TValueEvent>()
            .ToList();


        if (_line == null)
        {
            _line = new Polyline
            {
                Stroke = Stroke,
                StrokeThickness = 1,
                StrokeMiterLimit = 500,
                StrokeLineJoin = PenLineJoin.Bevel
            };

            AddElement(_line);
        }

        _line.Points.Clear();

        var localMin = UseAutoScale ? double.NaN : _min;
        var localMax = UseAutoScale ? double.NaN : _max;

        var valueToDisplay = double.NaN;

        if (events.Count > 0)
            for (var i = events.Count - 1; i >= 0; i--)
            {
                var @event = events[i];
                var x = GetXCoordinate(@event.DateTime);

                if (CheckXIsExpired(x) || _displayedEvents.Contains(@event))
                    break;

                _displayedEvents.Insert(0, @event);
            }

        if (_displayedEvents.Count == 0)
            return;

        var expiredEvents = new List<TValueEvent>();
        for (var i = 0; i < _displayedEvents.Count; i++)
        {
            var @event = _displayedEvents[i];
            var x = GetXCoordinate(@event.DateTime);

            if (CheckXIsExpired(x))
            {
                expiredEvents.Add(@event);
                continue;
            }

            var eventValue = GetEventValue(@event);

            if (localMin > eventValue || double.IsNaN(localMin))
                localMin = eventValue;

            if (localMax < eventValue || double.IsNaN(localMax))
                localMax = eventValue;

            valueToDisplay = eventValue;
            var y = CalculateYCoordinate(_min, _max, eventValue);
            _line.Points.Add(new Point(x, y));
        }

        _min = localMin;
        _max = localMax;

        _displayedEvents.RemoveAll(x => expiredEvents.Contains(x));

        DrawValue(valueToDisplay);
    }

    public void DrawValue(double value)
    {
        if (double.IsNaN(value))
            return;

        if (_valueLabel == null)
        {
            _valueLabel = new Label
            {
                Foreground = Stroke,
                FontSize = 14
            };

            AddElement(_valueLabel);
        }

        _valueLabel.Content = value;
        var x = 50d;
        var y = CalculateYCoordinate(_min, _max, value);

        SetElementCoordinates(_valueLabel, new Point(x, y));
    }

    protected abstract double GetEventValue(TValueEvent @event);
}