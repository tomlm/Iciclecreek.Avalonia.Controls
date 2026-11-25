using Avalonia;
using Avalonia.Controls;
using Avalonia.Headless;
using Avalonia.Headless.NUnit;
using Avalonia.Layout;
using Iciclecreek.Avalonia.Controls;
using Iciclecreek.Avalonia.Controls.Tests;

[assembly: AvaloniaTestApplication(typeof(TestApp))]

namespace Iciclecreek.Avalonia.Controls.Tests;

public static class TestApp
{
    public static AppBuilder BuildAvaloniaApp() =>
        AppBuilder.Configure<Application>().UseHeadless(new AvaloniaHeadlessPlatformOptions());
}

public class ColumnsPanelTests
{
    /// <summary>Lays the panel out in the given width, as tall as it asks to be.</summary>
    private static ColumnsPanel LaidOut(double width, ColumnsPanel panel, params double[] heights)
    {
        foreach (var height in heights)
            panel.Children.Add(new Border { Height = height });
        panel.Measure(new Size(width, double.PositiveInfinity));
        panel.Arrange(new Rect(0, 0, width, panel.DesiredSize.Height));
        return panel;
    }

    private static Rect[] Places(ColumnsPanel panel) => panel.Children.Select(c => c.Bounds).ToArray();

    [AvaloniaTest]
    public void EachChild_GoesInTheShortestColumn_AsWideAsTheColumn()
    {
        var panel = LaidOut(320, new ColumnsPanel { ColumnWidth = 100, ColumnGap = 10, Gap = 5 }, 50, 20, 30, 10);

        Assert.That(Places(panel), Is.EqualTo(new[]
        {
            new Rect(0, 0, 100, 50),
            new Rect(110, 0, 100, 20),
            new Rect(220, 0, 100, 30),
            // The middle column is the shortest: 20, then the gap.
            new Rect(110, 25, 100, 10),
        }));
    }

    [AvaloniaTest]
    public void ItsHeight_IsTheTallestColumns_WithGapsOnlyBetweenChildren()
    {
        var panel = LaidOut(320, new ColumnsPanel { ColumnWidth = 100, ColumnGap = 10, Gap = 5 }, 50, 20, 30, 10);

        Assert.That(panel.DesiredSize, Is.EqualTo(new Size(320, 50)));
    }

    [AvaloniaTest]
    public void Empty_ItTakesNoHeight()
    {
        var panel = LaidOut(320, new ColumnsPanel { ColumnWidth = 100, Gap = 5 });

        Assert.That(panel.DesiredSize.Height, Is.Zero);
    }

    [AvaloniaTest]
    public void AsManyColumnsAsFit_WithNoGapAfterTheLast()
    {
        var exact = LaidOut(320, new ColumnsPanel { ColumnWidth = 100, ColumnGap = 10 }, 10, 10, 10, 10);
        var less = LaidOut(319, new ColumnsPanel { ColumnWidth = 100, ColumnGap = 10 }, 10, 10, 10, 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Places(exact).Select(r => r.X).Distinct(), Is.EqualTo(new[] { 0d, 110, 220 }));
            Assert.That(Places(less).Select(r => r.X).Distinct(), Is.EqualTo(new[] { 0d, 110 }));
        }
    }

    [AvaloniaTest]
    public void WithinMaxColumns_AndWidthOfThoseUsed()
    {
        var panel = LaidOut(1000, new ColumnsPanel { ColumnWidth = 100, ColumnGap = 10, MaxColumns = 2 }, 10, 10, 10);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Places(panel).Select(r => r.X).Distinct(), Is.EqualTo(new[] { 0d, 110 }));
            Assert.That(panel.DesiredSize.Width, Is.EqualTo(210), "so it can be centered, or put to one side");
        }
    }

    [AvaloniaTest]
    public void TooNarrowForMinColumns_TheColumnsShareTheRoom_AndChildrenWrapToIt()
    {
        var panel = new ColumnsPanel { ColumnWidth = 100, ColumnGap = 10, MinColumns = 2 };
        var text = new TextBlock { Text = "a few words that wrap", TextWrapping = global::Avalonia.Media.TextWrapping.Wrap };
        panel.Children.Add(text);
        panel.Children.Add(new Border { Height = 10 });
        panel.Measure(new Size(150, double.PositiveInfinity));
        panel.Arrange(new Rect(0, 0, 150, panel.DesiredSize.Height));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(Places(panel).Select(r => (r.X, r.Width)), Is.EqualTo(new[] { (0d, 70d), (80d, 70d) }));
            Assert.That(text.Bounds.Width, Is.LessThanOrEqualTo(70), "measured at the narrower width, not clipped to it");
        }
    }

    [AvaloniaTest]
    public void ColumnDefinitions_ShareWhatTheFixedColumnsAndGapsLeave()
    {
        var panel = LaidOut(410, new ColumnsPanel { ColumnDefinitions = "*, 2*, 100", ColumnGap = 5 }, 10, 10, 10);

        Assert.That(Places(panel).Select(r => (r.X, r.Width)), Is.EqualTo(new[] { (0d, 100d), (105d, 200d), (310d, 100d) }));
    }

    [AvaloniaTest]
    public void AChildNarrowerThanItsColumn_IsPlacedByItsAlignment()
    {
        var panel = new ColumnsPanel { ColumnWidth = 100 };
        var child = new Border { Width = 40, Height = 10, HorizontalAlignment = HorizontalAlignment.Right };
        panel.Children.Add(child);
        panel.Measure(new Size(100, double.PositiveInfinity));
        panel.Arrange(new Rect(0, 0, 100, panel.DesiredSize.Height));

        Assert.That(child.Bounds, Is.EqualTo(new Rect(60, 0, 40, 10)));
    }
}
