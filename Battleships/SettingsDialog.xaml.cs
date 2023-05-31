using System;
using System.Windows;
using GameModel;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        public SettingsDialog(Settings settings)
        {
            InitializeComponent();
            (DataContext as SettingsViewModel)!.Settings = settings;
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
