﻿using Avalonia.Controls;
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
        public static readonly StyledProperty<double> ColumnGapProperty = AvaloniaProperty.Register<ColumnsPanel, double>(nameof(Gap), 0.0);

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
        /// Measures the control.
        /// </summary>
        /// <param name="availableSize">The available size.</param>
        /// <returns>The desired size of the control.</returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            double childAvailableWidth = double.PositiveInfinity;
            double childAvailableHeight = double.PositiveInfinity;

            childAvailableWidth = availableSize.Width;

            if (!double.IsNaN(Width))
            {
                childAvailableWidth = Width;
            }

            childAvailableWidth = Math.Min(childAvailableWidth, MaxWidth);
            childAvailableWidth = Math.Max(childAvailableWidth, MinWidth);

            var nColumns = GetNumberColumns(childAvailableWidth);

            var columnWidths = GetColumnWidths(childAvailableWidth, nColumns);

            var columnHeights = new List<double>();
            for (int i = 0; i < nColumns; i++)
                columnHeights.Add((double)0);

            foreach (Control child in Children)
            {
                var minCol = columnHeights.IndexOfMin();
                child.Measure(new Size(columnWidths[minCol], childAvailableHeight));
                columnHeights[minCol] += child.DesiredSize.Height + Gap;
            }

            var measuredHeight = columnHeights.Max() - Gap;
            return new Size(childAvailableWidth, measuredHeight * 2);
        }


        /// <summary>
        /// Arranges the control's children.
        /// </summary>
        /// <param name="finalSize">The size allocated to the control.</param>
        /// <returns>The space taken.</returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            double arrangedWidth = finalSize.Width;
            double arrangedHeight = finalSize.Height;
            double gap = Gap;

            arrangedHeight = 0;
            int nColumns = GetNumberColumns(arrangedWidth);

            var columnHeights = new List<double>(nColumns);
            var columnLefts = new List<double>(nColumns);
            var columnWidths = GetColumnWidths(finalSize.Width, nColumns);
            for (int i = 0; i < nColumns; i++)
            {
                columnHeights.Add((double)0);
                columnLefts.Add((double)0);
            }

            double left = 0;
            double scale = 1.0;
            if (nColumns == MinColumns && arrangedWidth / MinColumns < ColumnWidth)
            {
                scale = arrangedWidth / (MinColumns * (ColumnWidth + ColumnGap));
            }

            for (int iCol = 0; iCol < nColumns; iCol++)
            {
                if (iCol > 0)
                {
                    columnLefts[iCol] = left;
                }

                left += (columnWidths[iCol] * scale) + ColumnGap;
            }

            foreach (Control child in Children)
            {
                double childWidth = child.DesiredSize.Width * scale;
                double childHeight = child.DesiredSize.Height * scale;

                //  double width = Math.Max(childWidth, arrangedWidth);
                var minCol = columnHeights.IndexOfMin();
                Rect childFinal = new Rect(columnLefts[minCol], columnHeights[minCol], childWidth, childHeight);
                child.Arrange(childFinal);
                columnHeights[minCol] += childHeight + Gap;
            }

            arrangedHeight = Math.Max(arrangedHeight - gap, finalSize.Height);
            return new Size(arrangedWidth, arrangedHeight * 2);
        }

        private int GetNumberColumns(double arrangedWidth)
        {
            if (!String.IsNullOrEmpty(ColumnDefinitions))
            {
                var parts = ColumnDefinitions
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                return parts.Count;
            }

            var nColumns = Math.Max(MinColumns, (int)Math.Floor(arrangedWidth / (ColumnWidth + ColumnGap)));
            nColumns = Math.Min(MaxColumns, nColumns);
            return nColumns;
        }

        private List<double> GetColumnWidths(double childAvailableWidth, int nColumns)
        {
            List<double> columnWidths = new List<double>();
            if (!String.IsNullOrEmpty(ColumnDefinitions))
            {
                var autoValue = (ColumnWidth > 0) ? ColumnWidth.ToString() : "*";
                var parts = ColumnDefinitions
                    .Replace("Auto", autoValue, StringComparison.OrdinalIgnoreCase)
                    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
                int starCount = 0;
                for (int iCol = 0; iCol < nColumns; iCol++)
                {
                    if (parts[iCol].EndsWith('*'))
                    {
                        // it's a ratio star value
                        starCount++;
                        columnWidths.Add(0);
                    }
                    else
                    {
                        // it's a static value
                        if (float.TryParse(parts[iCol], out float width))
                        {
                            columnWidths.Add(width);
                        }
                        else
                        {
                            columnWidths.Add(0);
                        }
                    }
                }

                // var fixed parts
                var fixedWidth = columnWidths.Sum();
                float totalWeight = 0;
                for (int iCol = 0; iCol < nColumns; iCol++)
                {
                    if (parts[iCol].EndsWith("*"))
                    {
                        var part = parts[iCol].Trim('*');
                        if (String.IsNullOrEmpty(part))
                            part = "1";

                        if (float.TryParse(part, out var starWidth))
                        {
                            totalWeight += starWidth;
                            columnWidths[iCol] = starWidth;
                        }
                        else
                        {
                            columnWidths[iCol] = 0;
                        }
                    }
                }
                var available = childAvailableWidth - fixedWidth - (ColumnGap * nColumns - 1);
                for (int iCol = 0; iCol < nColumns; iCol++)
                {
                    if (parts[iCol].Contains("*"))
                    {
                        // this is the actual comupted relative width.
                        columnWidths[iCol] = columnWidths[iCol] * available / totalWeight;
                    }
                }
            }
            else
            {

                for (int iCol = 0; iCol < nColumns; iCol++)
                    columnWidths.Add((double)ColumnWidth);
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
