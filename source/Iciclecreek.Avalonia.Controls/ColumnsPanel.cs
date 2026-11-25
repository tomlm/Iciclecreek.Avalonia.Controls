using Avalonia.Controls;
using System;
using Avalonia;
using Avalonia.Input;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Design;
using AV = Avalonia.Controls;
using System.Runtime.CompilerServices;

namespace Iciclecreek.Avalonia.Controls
{
    /// <summary>
    /// A panel which lays out its children into fixed width columns with variable height items.
    /// </summary>
    /// <remarks>
    /// * ColumnWidth defines the fixed width of each column.
    /// * MaxColumns and MinColumns are used to determine the number of columns to layout.
    /// * Gap defines the space between items in the columns
    /// * ColumnGap defines the space between columns
    /// </remarks>
    public class ColumnsPanel : Panel, INavigableContainer
    {
        /// <summary>
        /// Defines the <see cref="Gap"/> property.
        /// </summary>
        public static readonly StyledProperty<double> GapProperty = AvaloniaProperty.Register<ColumnsPanel, double>(nameof(Gap), 0);

        /// <summary>
        /// Defines the <see cref="MinColumns"/> property.
        /// </summary>
        public static readonly StyledProperty<int> MinColumnsProperty = AvaloniaProperty.Register<ColumnsPanel, int>(nameof(MinColumns), 1);

        /// <summary>
        /// Defines the <see cref="MaxColumns"/> property.
        /// </summary>
        public static readonly StyledProperty<int> MaxColumnsProperty = AvaloniaProperty.Register<ColumnsPanel, int>(nameof(MaxColumns), int.MaxValue);

        /// <summary>
        /// Defines the <see cref="ColumnGap"/> property.
        /// </summary>
        public static readonly StyledProperty<double> ColumnGapProperty = AvaloniaProperty.Register<ColumnsPanel, double>(nameof(ColumnGap), 0.0);

        /// <summary>
        /// Defines the <see cref="ColumnWidthProperty"/> property.
        /// </summary>
        public static readonly StyledProperty<double> ColumnWidthProperty = AvaloniaProperty.Register<ColumnsPanel, double>(nameof(ColumnWidth), 300);

        /// <summary>
        /// Defines the <see cref="ColumnWidthProperty"/> property.
        /// </summary>
        public static readonly StyledProperty<string> ColumnDefinitionsProperty = AvaloniaProperty.Register<ColumnsPanel, string>(nameof(ColumnDefinitions));

        /// <summary>
        /// Initializes static members of the <see cref="ColumnsPanel"/> class.
        /// </summary>
        static ColumnsPanel()
        {
            AffectsMeasure<ColumnsPanel>(ColumnGapProperty);
            AffectsMeasure<ColumnsPanel>(ColumnWidthProperty);
            AffectsMeasure<ColumnsPanel>(GapProperty);
            AffectsMeasure<ColumnsPanel>(MaxColumnsProperty);
            AffectsMeasure<ColumnsPanel>(MinColumnsProperty);
            AffectsMeasure<ColumnsPanel>(ColumnDefinitionsProperty);
            // AffectsMeasure<ColumnsPanel2>(OrientationProperty);
        }

        /// <summary>
        /// Gets or sets the size of the gap to place between child controls.
        /// </summary>
        public double Gap
        {
            get { return GetValue(GapProperty); }
            set { SetValue(GapProperty, value); }
        }

        /// <summary>
        /// Gets or sets the size of the gap to place between columns
        /// </summary>
        public double ColumnGap
        {
            get { return GetValue(ColumnGapProperty); }
            set { SetValue(ColumnGapProperty, value); }
        }

        /// <summary>
        /// Gets or sets the column width for each column.
        /// </summary>
        public double ColumnWidth
        {
            get { return GetValue(ColumnWidthProperty); }
            set { SetValue(ColumnWidthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum number of columns desired
        /// </summary>
        public int MinColumns
        {
            get { return GetValue(MinColumnsProperty); }
            set { SetValue(MinColumnsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the maximum number of columns desired
        /// </summary>
        public int MaxColumns
        {
            get { return GetValue(MaxColumnsProperty); }
            set { SetValue(MaxColumnsProperty, value); }
        }

        /// <summary>
        /// Gets or sets the ColumnDefinitions
        /// </summary>
        public string ColumnDefinitions
        {
            get { return GetValue(ColumnDefinitionsProperty); }
            set { SetValue(ColumnDefinitionsProperty, value); }
        }

        /// <summary>
        /// Gets the next control in the specified direction.
        /// </summary>
        /// <param name="direction">The movement direction.</param>
        /// <param name="from">The control from which movement begins.</param>
        /// <returns>The control.</returns>
        public IInputElement? GetControl(NavigationDirection direction, IInputElement? from, bool wrap)
        {
            var fromControl = from as Control;
            return (fromControl != null) ? GetControlInDirection(direction, fromControl) : null;
        }

        /// <summary>
        /// Gets the next control in the specified direction.
        /// </summary>
        /// <param name="direction">The movement direction.</param>
        /// <param name="from">The control from which movement begins.</param>
        /// <returns>The control.</returns>
        protected virtual IInputElement GetControlInDirection(NavigationDirection direction, Control from)
        {
            int index = Children.IndexOf((Control)from);

            switch (direction)
            {
                case NavigationDirection.First:
                    index = 0;
                    break;
                case NavigationDirection.Last:
                    index = Children.Count - 1;
                    break;
                case NavigationDirection.Next:
                    ++index;
                    break;
                case NavigationDirection.Previous:
                    --index;
                    break;
                case NavigationDirection.Left:
                    index = -1;
                    break;
                case NavigationDirection.Right:
                    index = -1;
                    break;
                case NavigationDirection.Up:
                    index = index - 1;
                    break;
                case NavigationDirection.Down:
                    index = index + 1;
                    break;
                default:
                    index = -1;
                    break;
            }

            if (index >= 0 && index < Children.Count)
            {
                return Children[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Measures the control: each child at its column's width, in the shortest column so far.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The width the columns take, and the height of the tallest.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            var columnWidths = GetColumnWidths(availableSize.Width);
            var columnHeights = new double[columnWidths.Count];
            var columnItems = new int[columnWidths.Count];

            foreach (Control child in Children)
            {
                var column = columnHeights.IndexOfMin();
                child.Measure(new Size(columnWidths[column], double.PositiveInfinity));
                columnHeights[column] += (columnItems[column]++ > 0 ? Gap : 0) + child.DesiredSize.Height;
            }

            var usedWidth = columnWidths.Sum() + ColumnGap * Math.Max(0, columnWidths.Count - 1);
            return new Size(usedWidth, columnHeights.Max());
        }

        /// <summary>
        /// Arranges the control's children: each in the shortest column so far, as wide as its column.
        /// </summary>
        /// <param name="finalSize">The size allocated to the control.</param>
        /// <returns>The space taken.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            var columnWidths = GetColumnWidths(finalSize.Width);
            var columnHeights = new double[columnWidths.Count];
            var columnItems = new int[columnWidths.Count];
            var columnLefts = new double[columnWidths.Count];
            for (int iCol = 1; iCol < columnWidths.Count; iCol++)
                columnLefts[iCol] = columnLefts[iCol - 1] + columnWidths[iCol - 1] + ColumnGap;

            foreach (Control child in Children)
            {
                var column = columnHeights.IndexOfMin();
                var top = columnHeights[column] + (columnItems[column]++ > 0 ? Gap : 0);
                child.Arrange(new Rect(columnLefts[column], top, columnWidths[column], child.DesiredSize.Height));
                columnHeights[column] = top + child.DesiredSize.Height;
            }

            return finalSize;
        }

        /// <summary>
        /// The number of columns: one per column definition; or as many of <see cref="ColumnWidth"/>
        /// (with <see cref="ColumnGap"/> between them) as fit, within <see cref="MinColumns"/> and <see cref="MaxColumns"/>.
        /// </summary>
        private int GetNumberColumns(double availableWidth)
        {
            if (!String.IsNullOrEmpty(ColumnDefinitions))
            {
                return ColumnDefinitions
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .Length;
            }

            // n columns take n widths and n - 1 gaps: there is no gap after the last.
            var fit = double.IsInfinity(availableWidth) || ColumnWidth + ColumnGap <= 0
                ? MinColumns
                : (int)Math.Floor((availableWidth + ColumnGap) / (ColumnWidth + ColumnGap));
            return Math.Max(1, Math.Min(MaxColumns, Math.Max(MinColumns, fit)));
        }

        /// <summary>
        /// Each column's width: from the column definitions; or <see cref="ColumnWidth"/>, shared out
        /// narrower when <see cref="MinColumns"/> of them do not fit.
        /// </summary>
        private List<double> GetColumnWidths(double availableWidth)
        {
            var nColumns = GetNumberColumns(availableWidth);
            var gaps = ColumnGap * (nColumns - 1);
            List<double> columnWidths = new List<double>();
            if (!String.IsNullOrEmpty(ColumnDefinitions))
            {
                var autoValue = (ColumnWidth > 0) ? ColumnWidth.ToString() : "*";
                var parts = ColumnDefinitions
                    .Replace("Auto", autoValue, StringComparison.OrdinalIgnoreCase)
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                double totalWeight = 0;
                foreach (var part in parts)
                {
                    if (part.EndsWith('*'))
                    {
                        // A share of what the fixed columns leave.
                        var weight = part.TrimEnd('*');
                        var star = String.IsNullOrEmpty(weight) ? 1 : double.TryParse(weight, out var w) ? w : 0;
                        totalWeight += star;
                        columnWidths.Add(-star);
                    }
                    else
                    {
                        columnWidths.Add(double.TryParse(part, out var width) ? width : 0);
                    }
                }

                var fixedWidth = columnWidths.Where(w => w > 0).Sum();
                var available = double.IsInfinity(availableWidth)
                    ? ColumnWidth * columnWidths.Count(w => w < 0)
                    : Math.Max(0, availableWidth - fixedWidth - gaps);
                for (int iCol = 0; iCol < nColumns; iCol++)
                {
                    if (columnWidths[iCol] < 0)
                        columnWidths[iCol] = totalWeight > 0 ? -columnWidths[iCol] * available / totalWeight : 0;
                }
            }
            else
            {
                // Narrower than MinColumns columns need: each shares the room, so its children wrap to it.
                var width = double.IsInfinity(availableWidth)
                    ? ColumnWidth
                    : Math.Max(0, Math.Min(ColumnWidth, (availableWidth - gaps) / nColumns));
                for (int iCol = 0; iCol < nColumns; iCol++)
                    columnWidths.Add(width);
            }
            return columnWidths;
        }
    }

    public static class Extensions
    {
        public static int IndexOfMin(this IEnumerable<double> list)
        {
            var min = list.First();
            int minIndex = 0;

            int i = 0;
            foreach (var item in list)
            {
                if (item < min)
                {
                    min = item;
                    minIndex = i;
                }
                i++;
            }

            return minIndex;
        }

    }
}
