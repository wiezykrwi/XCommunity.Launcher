using System.Windows;

namespace SteamMods.Gui
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MinimizeButtonClicked(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void CloseButtonClicked(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}