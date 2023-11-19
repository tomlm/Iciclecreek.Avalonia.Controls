using Avalonia.Media;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sample.ViewModels;

public class SampleItem : ViewModelBase
{
    public SampleItem(string text)
    {
        Text = text;
        Height = 50 + new Random().Next(300);
        Color = Color.FromArgb(255, (byte)new Random().Next(255), (byte)new Random().Next(255), (byte)new Random().Next(255));
    }

    public string Text { get; set; }

    private int _width;
    public int Width { get=> _width; set => this.RaiseAndSetIfChanged(ref _width, value); }

    public int Height { get; set; }

    public Color Color { get; set; }
}


public class MainViewModel : ViewModelBase
{
    public MainViewModel()
    {
        Items = Enumerable.Range(1, 100).Select(i => new SampleItem($"Item {i}") { Width = ColumnWidth }).ToList();
    }


    public List<SampleItem> Items { get; set; }

    private int _columnWidth = 200;
    public int ColumnWidth
    {
        get => _columnWidth;
        set
        {
            this.RaiseAndSetIfChanged(ref _columnWidth, value);
            foreach (var item in Items)
            {
                item.Width = value;
            }
        }
    }

    private int _gap = 10;
    public int Gap { get => _gap; set => this.RaiseAndSetIfChanged(ref _gap, value); }

    private int _columnGap = 10;
    public int ColumnGap { get => _columnGap; set => this.RaiseAndSetIfChanged(ref _columnGap, value); }

    private int _minColumns = 1;
    public int MinColumns { get => _minColumns; set => this.RaiseAndSetIfChanged(ref _minColumns, value); }

    private int _maxColumns = 5;
    public int MaxColumns { get => _maxColumns; set => this.RaiseAndSetIfChanged(ref _maxColumns, value); }
}

