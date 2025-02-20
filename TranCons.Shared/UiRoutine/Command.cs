using System.Windows.Input;

namespace TranCons.Shared.UiRoutine;

public class Command<TParameter> : Command
    where TParameter : class
{
    public Command(Action<TParameter?> executeAction, Func<TParameter?, bool> canExecuteAction)
        : base(o => executeAction(o as TParameter),
            o => canExecuteAction(o as TParameter))
    {
    }

    public Command(Action<TParameter?> executeAction)
        : base(o => executeAction(o as TParameter))
    {
    }
}

public class Command : ICommand
{
    private readonly Action<object?> _executeAction;
    private readonly Func<object?, bool> _canExecuteAction;

    public Command(Action<object?> executeAction)
        : this(executeAction, null)
    {
    }

    public Command(Action<object?> executeAction, Func<object?, bool>? canExecuteAction)
    {
        _executeAction = executeAction;
        _canExecuteAction = canExecuteAction ?? (o => true);
    }

    public bool CanExecute(object? parameter)
    {
        return _canExecuteAction.Invoke(parameter);
    }

    public void Execute(object? parameter)
    {
        _executeAction.Invoke(parameter);
    }

    public event EventHandler? CanExecuteChanged;
}