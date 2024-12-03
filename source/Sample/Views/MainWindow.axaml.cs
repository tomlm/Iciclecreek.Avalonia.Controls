using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Layout;
using Iciclecreek.Avalonia.Controls;
using Sample.ViewModels;
using System;

namespace Sample.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnToggleSpinner(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainViewModel model = (MainViewModel)this.DataContext;
        model.SpinnerActive = !model.SpinnerActive;
    }
}
