using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TranCons.Shared.NetworkEvents;

namespace TranCons.Monitor.Controls;

public abstract class BaseGraphVisualizer : UserControl
{
    public static DependencyProperty EventProcessorTypeProperty = DependencyProperty.Register(nameof(EventProcessorType),
        typeof(Type),
        typeof(BaseGraphVisualizer));

    public static DependencyProperty EventsProperty = DependencyProperty.Register(nameof(Events),
        typeof(IReadOnlyCollection<IBaseEvent>),
        typeof(BaseGraphVisualizer));

    public Type EventProcessorType
    {
        get => (Type)GetValue(EventProcessorTypeProperty);
        set => SetValue(EventProcessorTypeProperty, value);
    }

    public IReadOnlyCollection<IBaseEvent> Events
    {
        get => (IReadOnlyCollection<IBaseEvent>)GetValue(EventsProperty);
        set => SetValue(EventsProperty, value);
    }

    public GraphPlot? Plot { get; set; }

    protected void AddElement(UIElement element)
    {
        Plot?.MasterCanvas.Children.Add(element);
    }
    protected void RemoveElement(UIElement element)
    {
        Plot?.MasterCanvas.Children.Remove(element);
    }

    protected double GetXCoordinate(DateTimeOffset moment)
    {
        return Plot?.GetXFromTime(moment) ?? double.NaN;
    }

    protected double CalculateYCoordinate(double min, double max, double value)
    {
        return Plot?.GetYFromValue(max, min, value) ?? double.NaN;
    }

    protected bool CheckXIsExpired(double x)
    {
        return Plot?.CheckXCoordinateExpired(x) ?? true;
    }

    protected void SetElementCoordinates(UIElement element, Point point)
    {
        SetElementXCoordinate(element, point.X);
        SetElementYCoordinate(element, point.Y);
    }

    protected void SetElementXCoordinate(UIElement element, double x)
    {
        Canvas.SetLeft(element, x);
    }

    protected void SetElementYCoordinate(UIElement element, double y)
    {
        Canvas.SetTop(element, y);
    }

    public void Draw()
    {
        if (Plot == null)
            return;

        DrawInternal();
    }

    protected abstract void DrawInternal();
}