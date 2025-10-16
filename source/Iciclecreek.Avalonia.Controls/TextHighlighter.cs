using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using System.Text.RegularExpressions;

namespace Iciclecreek.Avalonia.Controls
{
    /// <summary>
    /// A TextBlock control that supports highlighting text based on regex patterns.
    /// </summary>
    public class TextHighlighter : TextBlock
    {
        public static readonly StyledProperty<AvaloniaList<Highlighter>> HighlightersProperty =
            AvaloniaProperty.Register<TextHighlighter, AvaloniaList<Highlighter>>(nameof(Highlighters));

        public TextHighlighter()
        {
            Highlighters = new AvaloniaList<Highlighter>();
            Highlighters.CollectionChanged += (s, e) => UpdateHighlighting();
        }

        /// <summary>
        /// Gets or sets the collection of highlighters to apply to the text.
        /// </summary>
        public AvaloniaList<Highlighter> Highlighters
        {
            get => GetValue(HighlightersProperty);
            set => SetValue(HighlightersProperty, value);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == TextProperty || change.Property == HighlightersProperty)
            {
                UpdateHighlighting();
            }
        }

        private void UpdateHighlighting()
        {
            if (string.IsNullOrEmpty(Text) || Highlighters == null || Highlighters.Count == 0)
            {
                Inlines?.Clear();
                return;
            }

            var inlines = new InlineCollection();
            var segments = new List<TextSegment>();

            // Find all matches for all highlighters
            foreach (var highlighter in Highlighters)
            {
                if (highlighter?.Regex == null)
                    continue;

                var matches = highlighter.Regex.Matches(Text);
                foreach (Match match in matches)
                {
                    segments.Add(new TextSegment
                    {
                        Start = match.Index,
                        End = match.Index + match.Length,
                        Highlighter = highlighter
                    });
                }
            }

            // Sort segments by start position
            segments = segments.OrderBy(s => s.Start).ToList();

            // Merge overlapping segments (first highlighter takes precedence)
            var mergedSegments = new List<TextSegment>();
            foreach (var segment in segments)
            {
                if (mergedSegments.Count == 0 || segment.Start >= mergedSegments[mergedSegments.Count - 1].End)
                {
                    mergedSegments.Add(segment);
                }
                else
                {
                    // Handle overlapping segments - split if necessary
                    var lastSegment = mergedSegments[mergedSegments.Count - 1];
                    if (segment.End > lastSegment.End)
                    {
                        mergedSegments.Add(new TextSegment
                        {
                            Start = lastSegment.End,
                            End = segment.End,
                            Highlighter = segment.Highlighter
                        });
                    }
                }
            }

            // Build the inline collection
            int currentPosition = 0;
            foreach (var segment in mergedSegments)
            {
                // Add unhighlighted text before this segment
                if (segment.Start > currentPosition)
                {
                    inlines.Add(new Run(Text.Substring(currentPosition, segment.Start - currentPosition)));
                }

                // Add highlighted text
                var run = new Run(Text.Substring(segment.Start, segment.End - segment.Start));
                
                if (segment.Highlighter.Foreground != null)
                    run.Foreground = segment.Highlighter.Foreground;
                
                if (segment.Highlighter.Background != null)
                    run.Background = segment.Highlighter.Background;
                
                if (segment.Highlighter.FontWeight.HasValue)
                    run.FontWeight = segment.Highlighter.FontWeight.Value;
                
                if (segment.Highlighter.FontStyle.HasValue)
                    run.FontStyle = segment.Highlighter.FontStyle.Value;
                
                if (segment.Highlighter.TextDecorations != null)
                    run.TextDecorations = segment.Highlighter.TextDecorations;

                inlines.Add(run);
                currentPosition = segment.End;
            }

            // Add any remaining unhighlighted text
            if (currentPosition < Text.Length)
            {
                inlines.Add(new Run(Text.Substring(currentPosition)));
            }

            Inlines = inlines;
        }

        private class TextSegment
        {
            public int Start { get; set; }
            public int End { get; set; }
            public Highlighter Highlighter { get; set; }
        }
    }

    /// <summary>
    /// Defines a highlighting pattern and its visual properties.
    /// </summary>
    public class Highlighter : AvaloniaObject
    {
        public static readonly StyledProperty<string> PatternProperty =
            AvaloniaProperty.Register<Highlighter, string>(nameof(Pattern));

        public static readonly StyledProperty<IBrush> ForegroundProperty =
            AvaloniaProperty.Register<Highlighter, IBrush>(nameof(Foreground));

        public static readonly StyledProperty<IBrush> BackgroundProperty =
            AvaloniaProperty.Register<Highlighter, IBrush>(nameof(Background));

        public static readonly StyledProperty<FontWeight?> FontWeightProperty =
            AvaloniaProperty.Register<Highlighter, FontWeight?>(nameof(FontWeight));

        public static readonly StyledProperty<FontStyle?> FontStyleProperty =
            AvaloniaProperty.Register<Highlighter, FontStyle?>(nameof(FontStyle));

        public static readonly StyledProperty<TextDecorationCollection> TextDecorationsProperty =
            AvaloniaProperty.Register<Highlighter, TextDecorationCollection>(nameof(TextDecorations));

        private Regex _regex;

        public Highlighter()
        {
        }

        /// <summary>
        /// Gets or sets the regex pattern to match.
        /// </summary>
        public string Pattern
        {
            get => GetValue(PatternProperty);
            set => SetValue(PatternProperty, value);
        }

        /// <summary>
        /// Gets or sets the foreground brush for highlighted text.
        /// </summary>
        public IBrush Foreground
        {
            get => GetValue(ForegroundProperty);
            set => SetValue(ForegroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the background brush for highlighted text.
        /// </summary>
        public IBrush Background
        {
            get => GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        /// <summary>
        /// Gets or sets the font weight for highlighted text.
        /// </summary>
        public FontWeight? FontWeight
        {
            get => GetValue(FontWeightProperty);
            set => SetValue(FontWeightProperty, value);
        }

        /// <summary>
        /// Gets or sets the font style for highlighted text.
        /// </summary>
        public FontStyle? FontStyle
        {
            get => GetValue(FontStyleProperty);
            set => SetValue(FontStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the text decorations for highlighted text.
        /// </summary>
        public TextDecorationCollection TextDecorations
        {
            get => GetValue(TextDecorationsProperty);
            set => SetValue(TextDecorationsProperty, value);
        }

        internal Regex Regex
        {
            get
            {
                if (_regex == null && !string.IsNullOrEmpty(Pattern))
                {
                    try
                    {
                        _regex = new Regex(Pattern, RegexOptions.Compiled);
                    }
                    catch
                    {
                        // Invalid regex pattern
                        _regex = null;
                    }
                }
                return _regex;
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == PatternProperty)
            {
                _regex = null; // Force regex recompilation
            }
        }
    }
}