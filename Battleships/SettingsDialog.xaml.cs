using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
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
            // Hack to force window update
            var tempDataContext = (DataContext as SettingsViewModel);
            DataContext = null;
            tempDataContext!.Settings = settings;
            DataContext = tempDataContext;

        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
