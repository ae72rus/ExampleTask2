using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using TranCons.EventProcessors.Abstractions;
using TranCons.Monitor.Annotations;
using TranCons.Shared.NetworkEvents;

namespace TranCons.Monitor.ViewModels;

public class EventProcessorContainerViewModel : INotifyPropertyChanged, IDisposable
{
    private bool _isActive;
    private readonly IDisposable _subscription;

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive == value)
                return;

            _isActive = value;
            OnPropertyChanged(nameof(IsActive));
            ActiveChanged?.Invoke(this, _isActive);

            if (!_isActive)
                Events.Clear();
        }
    }

    public string Name { get; }
    public Type EventProcessorType { get; }
    public ObservableCollection<IBaseEvent> Events { get; set; } = new();
    protected int EventsBufferSize { get; set; } = 500;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event EventHandler<bool>? ActiveChanged;

    public EventProcessorContainerViewModel(IEventProcessor eventProcessor)
    {
        EventProcessorType = eventProcessor.GetType();
        Name = eventProcessor.Name;
        _subscription = eventProcessor.ObserveProcessedEvents().Subscribe(HandleEventInternal);
    }

    private void HandleEventInternal(IBaseEvent @event)
    {
        if (!_isActive)
            return;

        var localEvent = @event;//avoid closure
        Application.Current.Dispatcher.Invoke(() => HandleEvent(localEvent), DispatcherPriority.DataBind);
    }

    protected virtual void HandleEvent(IBaseEvent @event)
    {
        while (Events.Count >= EventsBufferSize)
        {
            Events.RemoveAt(0);
        }

        Events.Add(@event);
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public void Dispose()
    {
        _isActive = false;
        _subscription.Dispose();
        Events.Clear();
    }
}