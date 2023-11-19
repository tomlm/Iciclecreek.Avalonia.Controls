using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;

namespace Sample.ViewModels;

public class MainViewModel : ViewModelBase
{
    public class PicsumImage
    {
        public string? id { get; set; }
        public string? author { get; set; }
        public int? width { get; set; }
        public int? height { get; set; }
        public string? url { get; set; }
        public string? download_url { get; set; }
    }

    public MainViewModel()
    {
        Dispatcher.UIThread.Post(async () =>
        {
            HttpClient client = new HttpClient();
            var images = await client.GetFromJsonAsync<List<PicsumImage>>("https://picsum.photos/v2/list?limit=100");
            Random rnd = new Random();
            this.Images = images!.Select(pic =>
            {
                var width = pic.width / 10;
                var height = pic.height / 10;
                var url = pic.download_url!.Replace($"{pic.width}/{pic.height}", $"{width}/{height}");
                return url;
            }).OrderBy(x => rnd.Next())
            .ToList();
        });
    }

    public string Greeting => "Welcome to Avalonia!";

    private List<string> _images = new List<string>();
    public List<string> Images
    {
        get => _images; set
        {
            _images = value; 
            this.RaisePropertyChanged();
        }
    }

    private int _columnWidth = 200;
    public int ColumnWidth { get => _columnWidth; set => this.RaiseAndSetIfChanged(ref _columnWidth, value); }

    private int _gap = 10;
    public int Gap { get => _gap; set => this.RaiseAndSetIfChanged(ref _gap, value); }

    private int _columnGap = 10;
    public int ColumnGap { get => _columnGap ; set => this.RaiseAndSetIfChanged(ref _columnGap, value); }

    private int _minColumns = 1;
    public int MinColumns { get => _minColumns; set => this.RaiseAndSetIfChanged(ref _minColumns, value); }

    private int _maxColumns = 5;
    public int MaxColumns { get => _maxColumns; set => this.RaiseAndSetIfChanged(ref _maxColumns, value); }
}

