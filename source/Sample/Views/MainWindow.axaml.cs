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

        //foreach (var value in Enum.GetValues(typeof(AnimationType)))
        //{
        //    var panel = new StackPanel() { Orientation = Orientation.Horizontal };
        //    panel.Children.Add(new TextBlock()
        //    {
        //        Text = ((AnimationType)value).ToString(),
        //        FontFamily = new Avalonia.Media.FontFamily("Consolas"),
        //    });
        //    var spinner = new TextBlockSpinner()
        //    {
        //        AnimationType = (AnimationType)value,
        //        FontFamily = new Avalonia.Media.FontFamily("Consolas"),
        //        [!TextBlockSpinner.IsActiveProperty] = new Binding("SpinnerActive")
        //    };
        //    panel.Children.Add(spinner);
        //    this.SpinnerPanel.Children.Add(panel);
        //}
    }

    private void OnToggleSpinner(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        MainViewModel model = (MainViewModel)this.DataContext;
        model.SpinnerActive = !model.SpinnerActive;
    }
}
