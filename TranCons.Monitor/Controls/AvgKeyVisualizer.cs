using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TranCons.Shared.DTO;

namespace TranCons.Monitor.Controls;

public class AvgKeyVisualizer : BaseGraphVisualizer
{
    private Label? _avgKeyLabel;
    private InputEvent? _lastEvent;
    protected override void DrawInternal()
    {
        var lastEvent = Events.LastOrDefault() as InputEvent ?? _lastEvent;
        if (lastEvent == null)
            return;

        _lastEvent = lastEvent;

        if (_avgKeyLabel == null)
        {
            _avgKeyLabel = new Label
            {
                FontSize = 12,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                BorderThickness = new Thickness(0.5),
                BorderBrush = new SolidColorBrush(Colors.DarkGray),
                Background = Background
            };

            AddElement(_avgKeyLabel);
        }

        var x = GetXCoordinate(lastEvent.DateTime);
        var y = CalculateYCoordinate(0, 2, 1);


        if (CheckXIsExpired(x))
        {
            _avgKeyLabel.Visibility = Visibility.Collapsed;
            return;
        }

        _avgKeyLabel.Visibility = Visibility.Visible;

        _avgKeyLabel.Content = lastEvent.Key.ToString();
        SetElementCoordinates(_avgKeyLabel, new Point(x, y));
    }
}