using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using GameModel;

namespace Battleships
{
    internal class MessageDisplayers : IMessageDisplayer
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
