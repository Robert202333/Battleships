using System.Windows;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            (DataContext as GameViewModel)!.SetCanvas(boardCanvas);
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            (DataContext as GameViewModel)!.OnCanvasResize();
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            (DataContext as GameViewModel)!.StartNewGame();
        }
    }
}
