using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Extensions.Logging;
using TranCons.EventEmitter.Abstractions;
using TranCons.Shared.DTO;
using TranCons.Shared.UiRoutine;

namespace TranCons.Input.ViewModels;

public class InputWindowViewModel : INotifyPropertyChanged
{
    private readonly ILogger _logger;
    private readonly INetworkEventEmitter _eventEmitter;
    public event PropertyChangedEventHandler? PropertyChanged;

    public ICommand KeyUpCommand { get; }

    public InputWindowViewModel(ILogger logger,
        INetworkEventEmitter eventEmitter)
    {
        _logger = logger;
        _eventEmitter = eventEmitter;
        KeyUpCommand = new Command<KeyEventArgs>(OnKeyUp);
    }

    private async void OnKeyUp(KeyEventArgs? eventArgs)
    {
        if(eventArgs == null)
            return;

        await _eventEmitter.EmitEvent(new InputEvent((InputKey)eventArgs.Key, (InputKey)eventArgs.SystemKey));
        _logger.LogDebug($"Key Up event. Key: {eventArgs.Key}, System Key: {eventArgs.SystemKey}");
    }

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}