using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TranCons.EventProcessors.Abstractions;
using TranCons.Monitor.Annotations;
using System.Linq;

namespace TranCons.Monitor.ViewModels;

public class MonitorWindowViewModel : INotifyPropertyChanged, IDisposable
{
    private ObservableCollection<EventProcessorContainerViewModel> _activeEventProcessors = new();
    private bool _isDisposed;

    public event PropertyChangedEventHandler? PropertyChanged;
    public ObservableCollection<EventProcessorContainerViewModel> AvailableEventProcessors { get; }
    public ObservableCollection<EventProcessorContainerViewModel> ActiveEventProcessors
    {
        get => _activeEventProcessors;
        set
        {
            _activeEventProcessors = value;
            OnPropertyChanged(nameof(ActiveEventProcessors));
        }
    }

    public MonitorWindowViewModel(IEnumerable<IEventProcessor> eventProcessors)
    {
        AvailableEventProcessors = new ObservableCollection<EventProcessorContainerViewModel>(
            eventProcessors
                .Select(x =>
                {
                    var container = new EventProcessorContainerViewModel(x);
                    container.ActiveChanged += OnContainerActiveChanged;
                    return container;
                })
            );
    }

    private void OnContainerActiveChanged(object? sender, bool e)
    {
        if (_isDisposed)
            throw new ObjectDisposedException($"{nameof(MonitorWindowViewModel)} has been disposed");

        if (e)
            AddEventProcessor((EventProcessorContainerViewModel)sender!);
        else
            RemoveEventProcessor((EventProcessorContainerViewModel)sender!);
    }

    private void AddEventProcessor(EventProcessorContainerViewModel eventProcessor)
    {
        if (_activeEventProcessors.Contains(eventProcessor))
            return;

        _activeEventProcessors.Add(eventProcessor);
    }

    private void RemoveEventProcessor(EventProcessorContainerViewModel eventProcessor)
    {
        if (!_activeEventProcessors.Contains(eventProcessor))
            return;

        _activeEventProcessors.Remove(eventProcessor);
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        _isDisposed = true;
        foreach (var eventProcessor in AvailableEventProcessors)
        {
            eventProcessor.ActiveChanged -= OnContainerActiveChanged;
            eventProcessor.Dispose();
        }

        AvailableEventProcessors.Clear();
    }
}