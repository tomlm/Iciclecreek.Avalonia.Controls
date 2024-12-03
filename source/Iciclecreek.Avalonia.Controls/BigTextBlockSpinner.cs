using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Threading;

namespace Iciclecreek.Avalonia.Controls
{

    public class BigTextBlockSpinner : Grid
    {
        public static readonly StyledProperty<int> SpeedProperty =
            AvaloniaProperty.Register<TextBlockSpinner, int>(nameof(Speed), defaultValue: 50);

        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<TextBlockSpinner, bool>(nameof(IsActive), defaultValue: false);

        public static readonly StyledProperty<BigAnimationType> AnimationTypeProperty =
            AvaloniaProperty.Register<TextBlockSpinner, BigAnimationType>(nameof(AnimationType), defaultValue: BigAnimationType.Dots);

        public static readonly StyledProperty<int> ColumnsProperty =
            AvaloniaProperty.Register<TextBlockSpinner, int>(nameof(Columns), defaultValue: 2);

        public static readonly StyledProperty<int> RowsProperty =
            AvaloniaProperty.Register<TextBlockSpinner, int>(nameof(Rows), defaultValue: 2);

        public static readonly StyledProperty<IBrush> ForegroundProperty =
            AvaloniaProperty.Register<TextBlockSpinner, IBrush>(nameof(Foreground), defaultValue: Brushes.White);


        public static readonly StyledProperty<int> LengthProperty =
            AvaloniaProperty.Register<TextBlockSpinner, int>(nameof(Length), defaultValue: 3);

        private CancellationTokenSource _cancelationTokenSource;
        private int _frame;
        private Task _spinnerTask;
        private List<Point> _frames = new List<Point>();

        public BigTextBlockSpinner()
        {
            this.Initialized += BigTextBlockSpinner_Initialized;
            this.Margin = new Thickness(0);
            this.IsVisible = false;
        }

        private void BigTextBlockSpinner_Initialized(object sender, EventArgs e)
        {
            LoadAnimation();
        }

        protected async override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            switch (change.Property.Name)
            {
                case nameof(IsActive):
                    if (IsActive)
                    {
                        StopSpinner();

                        _spinnerTask = StartSpinner();
                        this.IsVisible = true;
                    }
                    else
                    {
                        StopSpinner();
                        this.IsVisible = false;
                    }
                    break;

                case nameof(AnimationType):
                    LoadAnimation();
                    break;

                default:
                    base.OnPropertyChanged(change);
                    break;
            }
        }

        public int Speed
        {
            get { return GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        /// <summary>
        /// If true the spinner is visible and animated, if false the spinner is hidden and not animated.
        /// </summary>
        public bool IsActive
        {
            get { return GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }

        public BigAnimationType AnimationType
        {
            get { return GetValue(AnimationTypeProperty); }
            set { SetValue(AnimationTypeProperty, value); }
        }

        public int Columns
        {
            get { return GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public int Rows
        {
            get { return GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        public IBrush Foreground
        {
            get { return GetValue(ForegroundProperty); }
            set { SetValue(ForegroundProperty, value); }
        }

        /// <summary>
        /// Lenght of the tail on the spinner
        /// </summary>
        public int Length
        {
            get { return GetValue(LengthProperty); }
            set { SetValue(LengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the font family used to draw the control's text.
        /// </summary>
        public FontFamily FontFamily
        {
            get => GetValue(TextBlock.FontFamilyProperty);
            set => SetValue(TextBlock.FontFamilyProperty, value);
        }

        /// <summary>
        /// Gets or sets the size of the control's text in points.
        /// </summary>
        public double FontSize
        {
            get => GetValue(TextBlock.FontSizeProperty);
            set => SetValue(TextBlock.FontSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets the font style used to draw the control's text.
        /// </summary>
        public FontStyle FontStyle
        {
            get => GetValue(TextBlock.FontStyleProperty);
            set => SetValue(TextBlock.FontStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the font weight used to draw the control's text.
        /// </summary>
        public FontWeight FontWeight
        {
            get => GetValue(TextBlock.FontWeightProperty);
            set => SetValue(TextBlock.FontWeightProperty, value);
        }
        private async Task StartSpinner()
        {
            StopSpinner();

            LoadAnimation();

            _cancelationTokenSource = new CancellationTokenSource();
            while (_cancelationTokenSource.IsCancellationRequested == false)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Point[] points = new Point[Length];
                    var index = _frame++;
                    for (int count = Length-1; count >= 0; count--)
                    {
                        points[count] = _frames[index++ % _frames.Count];
                    }

                    foreach (var tb in Children.Cast<TextBlock>())
                    {
                        var x = Grid.GetColumn(tb);
                        var y = Grid.GetRow(tb);
                        if (points.Any(p => p.X == x && p.Y == y))
                        {
                            tb.Foreground = this.Foreground;
                        }
                        else
                        {
                            tb.Foreground = Brushes.Transparent;
                        }
                    }
                });
                try
                {

                    await Task.Delay(Speed, _cancelationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
            }
        }

        private void LoadAnimation()
        {
            Children.Clear();
            _frames.Clear();
            var length = 2 * ((Columns - 1) + (Rows - 1));

            for (int x = 0; x < Columns; x++)
            {
                this.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
            }
            for (int y = 0; y < Rows; y++)
            {
                this.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            }

            // create the textblocks
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    string text;
                    if (x == 0 && y == 0)
                    {
                        if (AnimationType == BigAnimationType.SquareLines)
                            text = "┌";
                        else if (AnimationType == BigAnimationType.RoundLines)
                            text = "╭";
                        else
                            text = "◦";
                    }
                    else if (x == Columns - 1 && y == 0)
                    {
                        if (AnimationType == BigAnimationType.SquareLines)
                            text = "┐";
                        else if (AnimationType == BigAnimationType.RoundLines)
                            text = "╮";
                        else
                            text = "◦";
                    }
                    else if (x == 0 && y == Rows - 1)
                    {
                        if (AnimationType == BigAnimationType.SquareLines)
                            text = "└";
                        else if (AnimationType == BigAnimationType.RoundLines)
                            text = "╰";
                        else
                            text = "◦";
                    }
                    else if (x == Columns - 1 && y == Rows - 1)
                    {
                        if (AnimationType == BigAnimationType.SquareLines)
                            text = "┘";
                        else if (AnimationType == BigAnimationType.RoundLines)
                            text = "╯";
                        else
                            text = "◦";
                    }
                    else if (x == 0 || x == Columns - 1)
                    {
                        if (AnimationType == BigAnimationType.SquareLines)
                            text = "│";
                        else if (AnimationType == BigAnimationType.RoundLines)
                            text = "│";
                        else
                            text = "◦";
                    }
                    else if (y == 0 || y == Rows - 1)
                    {
                        if (AnimationType == BigAnimationType.SquareLines)
                            text = "─";
                        else if (AnimationType == BigAnimationType.RoundLines)
                            text = "─";
                        else
                            text = "◦";
                    }
                    else
                        text = " ";

                    var textBlock = new TextBlock()
                    {
                        DataContext = text,
                        Text = text,
                        FontFamily = this.FontFamily ?? new FontFamily("Consolas"),
                        FontSize = this.FontSize,
                        FontStyle = this.FontStyle,
                        FontWeight = this.FontWeight,
                        Foreground = Brushes.Transparent,
                        Background = Brushes.Transparent
                    };
                    Grid.SetColumn(textBlock, x);
                    Grid.SetRow(textBlock, y);
                    Children.Add(textBlock);
                }
            }
            for (int x = 0; x < Columns; x++)
                _frames.Add(new Point(x, 0));

            for (int y = 1; y < Rows; y++)
                _frames.Add(new Point(Columns - 1, y));

            for (int x = Columns - 2; x >= 0; x--)
                _frames.Add(new Point(x, Rows - 1));

            for (int y = Rows - 2; y > 0; y--)
                _frames.Add(new Point(0, y));

            _frame = 0;
        }

        private void StopSpinner()
        {
            if (_cancelationTokenSource != null)
            {
                _cancelationTokenSource.Cancel();

                if (_spinnerTask != null)
                {
                    _spinnerTask = null;
                }
            }
        }
    }
}
