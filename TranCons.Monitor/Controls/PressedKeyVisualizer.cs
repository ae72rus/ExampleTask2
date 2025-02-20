using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TranCons.Shared.DTO;

namespace TranCons.Monitor.Controls;

public class PressedKeyVisualizer : BaseGraphVisualizer
{
    private readonly Dictionary<InputEvent, Label> _labelEvents = new();

    protected override void DrawInternal()
    {
        var y = 50d;
        var events = Events.ToList().OfType<InputEvent>();
        foreach (var @event in events)
        {
            var x = GetXCoordinate(@event.DateTime);
            if (CheckXIsExpired(x))
                continue;

            if (_labelEvents.ContainsKey(@event))
                continue;

            var conflictLabels = _labelEvents.Values
                .Where(e => e.ActualWidth + Canvas.GetLeft(e) >= x).ToArray();

            if (conflictLabels.Length > 0)
                y = conflictLabels
                    .Max(e => Canvas.GetBottom(e) + e.ActualHeight);

            var label = _labelEvents[@event] = new Label
            {
                Content = @event.Key.ToString(),
                FontSize = 12,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                BorderThickness = new Thickness(0.5),
                BorderBrush = new SolidColorBrush(Colors.DarkGray),
                Background = Background
            };

            AddElement(label);
            Canvas.SetBottom(label, y);
        }

        foreach (var labelEvent in _labelEvents.ToArray())
        {
            var label = labelEvent.Value;
            var x = GetXCoordinate(labelEvent.Key.DateTime);
            if (CheckXIsExpired(x))
            {
                RemoveElement(label);
                _labelEvents.Remove(labelEvent.Key);
                continue;
            }

            label.Visibility = Visibility.Visible;
            SetElementXCoordinate(label, x);
        }
    }
}