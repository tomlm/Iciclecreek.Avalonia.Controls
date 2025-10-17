using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Media;
using System.Text.RegularExpressions;

namespace Iciclecreek.Avalonia.Controls
{
    public class HighlighterCollection : AvaloniaList<Highlighter>
    {
    }

    /// <summary>
    /// A TextBlock control that supports highlighting text based on regex patterns.
    /// </summary>
    public class TextHighlighter : TextBlock
    {
        public static readonly StyledProperty<HighlighterCollection> HighlightersProperty =
            AvaloniaProperty.Register<TextHighlighter, HighlighterCollection>(nameof(Highlighters));

        public TextHighlighter()
        {
            Highlighters = new HighlighterCollection();
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
            
            // Create a list of all match positions with their highlighters
            var matches = new List<HighlightMatch>();
            
            // Track highlighters with HighlightAll=true that have matches
            var highlightAllHighlighters = new List<Highlighter>();
            
            foreach (var highlighter in Highlighters)
            {
                if (highlighter?.Regex == null)
                    continue;

                var regexMatches = highlighter.Regex.Matches(Text);
                
                if (highlighter.HighlightAll && regexMatches.Count > 0)
                {
                    // If HighlightAll is true and pattern matches, track this highlighter
                    highlightAllHighlighters.Add(highlighter);
                }
                else
                {
                    // Normal highlighting - only matched regions
                    foreach (Match match in regexMatches)
                    {
                        matches.Add(new HighlightMatch
                        {
                            Start = match.Index,
                            End = match.Index + match.Length,
                            Highlighter = highlighter
                        });
                    }
                }
            }

            if (matches.Count == 0 && highlightAllHighlighters.Count == 0)
            {
                inlines.Add(new Run(Text));
                Inlines = inlines;
                return;
            }

            // Create breakpoints at all segment boundaries
            var breakpoints = new SortedSet<int> { 0, Text.Length };
            foreach (var match in matches)
            {
                breakpoints.Add(match.Start);
                breakpoints.Add(match.End);
            }

            var breakpointList = breakpoints.ToList();

            // For each segment between breakpoints, determine which highlighters apply
            for (int i = 0; i < breakpointList.Count - 1; i++)
            {
                int start = breakpointList[i];
                int end = breakpointList[i + 1];
                
                // Find all highlighters that apply to this segment
                var applicableHighlighters = matches
                    .Where(m => m.Start <= start && m.End >= end)
                    .Select(m => m.Highlighter)
                    .ToList();

                // Add HighlightAll highlighters (they apply to the entire text)
                applicableHighlighters.AddRange(highlightAllHighlighters);

                if (applicableHighlighters.Count == 0)
                {
                    // No highlighting for this segment
                    inlines.Add(new Run(Text.Substring(start, end - start)));
                }
                else
                {
                    // Apply attributes from highlighters (first one wins for each attribute)
                    var run = new Run(Text.Substring(start, end - start));

                    // Apply Foreground (first highlighter with Foreground wins)
                    var foregroundHighlighter = applicableHighlighters.FirstOrDefault(h => h.Foreground != null);
                    if (foregroundHighlighter != null)
                        run.Foreground = foregroundHighlighter.Foreground;

                    // Apply Background (first highlighter with Background wins)
                    var backgroundHighlighter = applicableHighlighters.FirstOrDefault(h => h.Background != null);
                    if (backgroundHighlighter != null)
                        run.Background = backgroundHighlighter.Background;

                    // Apply FontWeight (first highlighter with FontWeight wins)
                    var fontWeightHighlighter = applicableHighlighters.FirstOrDefault(h => h.FontWeight.HasValue);
                    if (fontWeightHighlighter != null)
                        run.FontWeight = fontWeightHighlighter.FontWeight.Value;

                    // Apply FontStyle (first highlighter with FontStyle wins)
                    var fontStyleHighlighter = applicableHighlighters.FirstOrDefault(h => h.FontStyle.HasValue);
                    if (fontStyleHighlighter != null)
                        run.FontStyle = fontStyleHighlighter.FontStyle.Value;

                    // Apply TextDecorations (first highlighter with TextDecorations wins)
                    var textDecorationsHighlighter = applicableHighlighters.FirstOrDefault(h => h.TextDecorations != null);
                    if (textDecorationsHighlighter != null)
                        run.TextDecorations = textDecorationsHighlighter.TextDecorations;

                    inlines.Add(run);
                }
            }

            Inlines = inlines;
        }

        private class HighlightMatch
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

        public static readonly StyledProperty<bool> HighlightAllProperty =
            AvaloniaProperty.Register<Highlighter, bool>(nameof(HighlightAll));

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

        /// <summary>
        /// Gets or sets whether to apply highlighting to the entire text when the pattern matches.
        /// If true, the highlighter's attributes apply to all text when the pattern is found anywhere.
        /// If false (default), only the matched text portions are highlighted.
        /// </summary>
        public bool HighlightAll
        {
            get => GetValue(HighlightAllProperty);
            set => SetValue(HighlightAllProperty, value);
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