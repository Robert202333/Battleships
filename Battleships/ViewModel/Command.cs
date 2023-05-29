using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Battleships
{
    public class Command : ICommand
    {
        private readonly Action<object?> executeAction;
        private readonly Func<object?, bool>? canExecuteAction;

        public Command(Action<object?> executeAction, Func<object?, bool>? canExecuteAction = null)
        {
            this.executeAction = executeAction;
            this.canExecuteAction = canExecuteAction;
        }

        public void Execute(object? parameter) => executeAction(parameter);

        public bool CanExecute(object? parameter) => canExecuteAction?.Invoke(parameter) ?? true;

        public event EventHandler? CanExecuteChanged;

        public void InvokeCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}
