using System;
using System.Windows;
using System.Windows.Input;
using Xcommunity.Launcher.Gui.ViewModels;

namespace Xcommunity.Launcher.Gui;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = ViewModelLocator.Instance.Main;
    }

    private void MinimizeButtonClicked(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseButtonClicked(object sender, RoutedEventArgs e)
    {
        try
        {
            (DataContext as MainViewModel)?.Save();
        }
        catch (Exception exception)
        {
            MessageBox.Show(exception.Message, "Error while saving mod metadata", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        Close();
    }

    private void MainWindow_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }
}