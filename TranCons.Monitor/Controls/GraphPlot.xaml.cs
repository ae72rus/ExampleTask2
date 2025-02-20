using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TranCons.Monitor.ViewModels;

namespace TranCons.Monitor.Controls
{
    /// <summary>
    /// Interaction logic for GraphPlotBase.xaml
    /// </summary>
    public partial class GraphPlot : UserControl
    {
        public static DependencyProperty SeriesProperty = DependencyProperty.Register(nameof(Series),
            typeof(IReadOnlyCollection<EventProcessorContainerViewModel>),
            typeof(GraphPlot),
            new PropertyMetadata(Array.Empty<EventProcessorContainerViewModel>()));

        public static DependencyProperty VisualizersProperty = DependencyProperty.Register(nameof(Visualizers),
            typeof(GraphVisualizerCollection),
            typeof(GraphPlot),
            new PropertyMetadata(new GraphVisualizerCollection()));

        private readonly Line _nowLine;

        private readonly double _plotSeconds = 10;
        private readonly double _plotBottomLimit = 50;
        private readonly double _plotTopLimit = 50;
        private readonly double _plotLeftLimit = 50;
        private readonly double _plotRightLimit = 50;
        private bool _renderGraphics;

        private readonly Dictionary<EventProcessorContainerViewModel, BaseGraphVisualizer> _visualizers = new();
        private readonly Dictionary<Line, DateTimeOffset> _verticalLines = new();
        private readonly List<Label> _verticalLinesLabels = new();

        private DateTimeOffset _moment;

        public IReadOnlyCollection<EventProcessorContainerViewModel> Series
        {
            get => (IReadOnlyCollection<EventProcessorContainerViewModel>)GetValue(SeriesProperty);
            set => SetValue(SeriesProperty, value);
        }

        public GraphVisualizerCollection Visualizers
        {
            get => (GraphVisualizerCollection)GetValue(VisualizersProperty);
            set => SetValue(VisualizersProperty, value);
        }

        public GraphPlot()
        {
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnload;

            _nowLine = new Line
            {
                StrokeThickness = 1,
                Stroke = new SolidColorBrush(Colors.Red)
            };
        }

        private void OnUnload(object sender, RoutedEventArgs e)
        {
            _renderGraphics = false;
            Loaded -= OnLoaded;
            Unloaded -= OnUnload;
        }

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var graphVisualizer in Visualizers)
                graphVisualizer.Plot = this;

            for (var i = 0; i < 25; i++)
            {
                var line = new Line
                {
                    StrokeThickness = 0.5,
                    Stroke = new SolidColorBrush(Colors.LightGray)
                };

                var label = new Label
                {
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    FontSize = 8
                };

                MasterCanvas.Children.Add(line);
                MasterCanvas.Children.Add(label);
                _verticalLines[line] = DateTimeOffset.MinValue;
                _verticalLinesLabels.Add(label);
            }

            DrawStaticAxes();
            MasterCanvas.Children.Add(_nowLine);

            _renderGraphics = true;
            while (_renderGraphics)
            {
                Draw();
                await Task.Delay(1);
            }
        }

        private void DrawStaticAxes()
        {
            var xAxis = new Line
            {
                StrokeThickness = 2,
                Stroke = new SolidColorBrush(Colors.Black),
                X1 = _plotLeftLimit,
                X2 = ActualWidth - _plotRightLimit,
                Y1 = ActualHeight - _plotBottomLimit,
                Y2 = ActualHeight - _plotBottomLimit
            };

            var yAxis = new Line
            {
                StrokeThickness = 2,
                Stroke = new SolidColorBrush(Colors.Black),
                X1 = _plotLeftLimit,
                X2 = _plotLeftLimit,
                Y1 = ActualHeight - _plotBottomLimit,
                Y2 = _plotTopLimit
            };

            MasterCanvas.Children.Add(xAxis);
            MasterCanvas.Children.Add(yAxis);
        }

        private void Draw()
        {
            _moment = DateTimeOffset.Now;
            DrawNowLine();
            DrawVerticalLines();
            DrawVerticalLinesLabels();
            DrawSeries();
        }

        public double GetXFromTime(DateTimeOffset time)
        {
            var zeroTime = _moment.AddSeconds(-_plotSeconds);
            var timSpanFromZeroToTime = (time - zeroTime).TotalSeconds;
            var plotWidth = ActualWidth - _plotLeftLimit - _plotRightLimit;
            return _plotLeftLimit + plotWidth * timSpanFromZeroToTime / _plotSeconds;
        }

        public double GetYFromValue(double maxValue, double minValue, double value)
        {
            if (minValue > value)
                minValue = value;

            if (maxValue < value)
                maxValue = value;

            var plotHeight = ActualHeight - _plotBottomLimit - _plotTopLimit;
            return ActualHeight - (_plotTopLimit + plotHeight * (value - minValue) / (maxValue > minValue ? maxValue - minValue : 1));
        }

        public bool CheckXCoordinateExpired(double x)
        {
            return _plotLeftLimit > x;
        }

        private void DrawNowLine()
        {
            var x = GetXFromTime(_moment);
            _nowLine.X1 = _nowLine.X2 = x;
            _nowLine.Y1 = 0;
            _nowLine.Y2 = ActualHeight;
        }

        private void DrawVerticalLines()//500 ms between lines
        {
            var rightLinePoint = new DateTimeOffset(
                _moment.Year,
                _moment.Month,
                _moment.Day,
                _moment.Hour,
                _moment.Minute,
                _moment.Second,
                0,
                _moment.Offset);

            for (var i = 0; i < _verticalLines.Count; i++)
            {
                var lineTime = rightLinePoint.AddMilliseconds(-500 * (i - 2));
                var x = GetXFromTime(lineTime);
                var line = _verticalLines.Keys.ElementAt(i);
                _verticalLines[line] = lineTime;

                if (x <= _plotLeftLimit || x >= ActualWidth - _plotRightLimit)
                {
                    line.Visibility = Visibility.Hidden;
                    continue;
                }

                line.Visibility = Visibility.Visible;

                line.X1 = x;
                line.X2 = x;
                line.Y2 = _plotBottomLimit;
                line.Y1 = ActualHeight - _plotTopLimit;
            }
        }

        private void DrawVerticalLinesLabels()
        {
            for (var i = 0; i < _verticalLines.Count; i++)
            {
                var keyValuePair = _verticalLines.ElementAt(i);
                var label = _verticalLinesLabels[i];
                label.Visibility = keyValuePair.Key.Visibility;

                label.Content = keyValuePair.Value.ToString("HH:mm:ss.f");
                Canvas.SetLeft(label, keyValuePair.Key.X1);
                Canvas.SetTop(label, ActualHeight - _plotBottomLimit / 2);
            }
        }

        private void DrawSeries()
        {
            foreach (var series in Series)
            {
                if (_visualizers.TryGetValue(series, out var visualizer))
                    continue;

                visualizer = Visualizers.FirstOrDefault(x => x.EventProcessorType == series.EventProcessorType);
                if (visualizer == null)
                    continue;

                visualizer.DataContext = series;
                _visualizers[series] = visualizer;
            }

            foreach (var visualizer in _visualizers.Values)
                visualizer.Draw();
        }
    }
}
