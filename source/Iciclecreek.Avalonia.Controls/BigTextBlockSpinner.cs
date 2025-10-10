using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using Avalonia.Layout;
using Avalonia.Controls.Documents;

namespace Iciclecreek.Avalonia.Controls
{

    public class BigTextBlockSpinner : StackPanel
    {
        public static readonly StyledProperty<int> SpeedProperty =
            AvaloniaProperty.Register<TextBlockSpinner, int>(nameof(Speed), defaultValue: 50);

        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<TextBlockSpinner, bool>(nameof(IsActive), defaultValue: false);

        public static readonly StyledProperty<char> AnimationCharProperty =
            AvaloniaProperty.Register<TextBlockSpinner, char>(nameof(AnimationChar), defaultValue: '◦');

        public static readonly StyledProperty<int> ColumnsProperty =
            AvaloniaProperty.Register<TextBlockSpinner, int>(nameof(Columns), defaultValue: 2);

        public static readonly StyledProperty<int> RowsProperty =
            AvaloniaProperty.Register<TextBlockSpinner, int>(nameof(Rows), defaultValue: 2);

        public static readonly StyledProperty<IBrush> ForegroundProperty =
            AvaloniaProperty.Register<TextBlockSpinner, IBrush>(nameof(Foreground), defaultValue: Brushes.White);

        public static readonly StyledProperty<int> LengthProperty =
            AvaloniaProperty.Register<TextBlockSpinner, int>(nameof(Length), defaultValue: 3);

        private int _frame;
        private List<Point> _frames = new List<Point>();

        public BigTextBlockSpinner()
        {
            this.Initialized += BigTextBlockSpinner_Initialized;
            this.Orientation = Orientation.Vertical;
            this.IsVisible = false;
        }

        private void BigTextBlockSpinner_Initialized(object sender, EventArgs e)
        {
            LoadAnimation();
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            switch (change.Property.Name)
            {
                case nameof(IsActive):
                    if (IsActive)
                    {
                        _ = StartSpinner();
                        this.IsVisible = true;
                    }
                    else
                    {
                        this.IsVisible = false;
                    }
                    break;

                case nameof(AnimationChar):
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

        public char AnimationChar
        {
            get { return GetValue(AnimationCharProperty); }
            set { SetValue(AnimationCharProperty, value); }
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
            LoadAnimation();

            while (IsActive)
            {
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Point[] points = new Point[Length];
                    for (int count = 0; count < Length; count++)
                    {
                        points[count] = _frames[(_frame + count) % _frames.Count];
                    }

                    for (int y = 0; y < Rows; y++)
                    {
                        var tb = (TextBlock)Children[y];
                        for (int x = 0; x < Columns; x++)
                        {
                            if (points.Any(p => p.X == x && p.Y == y))
                            {
                                tb.Inlines[x].Foreground = this.Foreground;
                            }
                            else
                            {
                                tb.Inlines[x].Foreground = Brushes.Transparent;
                            }
                        }
                    }
                    _frame++;
                    if (_frame >= _frames.Count)
                    {
                        _frame = 0;
                    }
                });

                await Task.Delay(Speed);
            }
        }

        private void LoadAnimation()
        {
            Children.Clear();
            _frames.Clear();
            var length = 2 * ((Columns - 1) + (Rows - 1));

            // create the textblocks
            for (int y = 0; y < Rows; y++)
            {
                var textBlock = new TextBlock()
                {
                    FontFamily = new FontFamily("Cascadia Mono"),
                    FontSize = this.FontSize,
                    FontStyle = this.FontStyle,
                    FontWeight = this.FontWeight,
                    Foreground = Brushes.Transparent,
                    Background = Brushes.Transparent
                };
                for (int x = 0; x < Columns; x++)
                {
                    string text;
                    if ((x == 0 && y == 0) ||
                        (x == Columns - 1 && y == 0) ||
                        (x == 0 && y == Rows - 1) ||
                        (x == Columns - 1 && y == Rows - 1) ||
                        (x == 0 || x == Columns - 1) ||
                        (y == 0 || y == Rows - 1))
                        text = AnimationChar.ToString();
                    else
                        text = " ";

                    textBlock.Inlines.Add(new Run(text)
                    {
                        Background = Brushes.Transparent,
                        Foreground = Brushes.Transparent
                    });
                }
                Children.Add(textBlock);
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

    }
}
