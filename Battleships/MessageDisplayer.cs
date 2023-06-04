using GameModel;
using System.Windows;

namespace Battleships
{
    internal class MessageDisplayer : IMessageDisplayer
    {
        public void ShowWarning(string type, string message)
        {
            MessageBox.Show(message, type, MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        public void ShowError(string type, string message)
        {
            MessageBox.Show(message, type, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public void ShowInformation(string type, string message)
        {
            MessageBox.Show(message, type, MessageBoxButton.OK, MessageBoxImage.Information);
        }

    }
}
