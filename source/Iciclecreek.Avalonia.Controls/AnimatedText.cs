using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Iciclecreek.Avalonia.Controls
{
    public class AnimatedText
    {
        public static readonly AttachedProperty<object?> FramesProperty =
            AvaloniaProperty.RegisterAttached<AnimatedText, TextBlock, object?>("Frames");

        public static readonly AttachedProperty<bool> IsAnimatingProperty =
            AvaloniaProperty.RegisterAttached<AnimatedText, TextBlock, bool>("IsAnimating");

        public static readonly AttachedProperty<AnimationType> AnimationProperty =
            AvaloniaProperty.RegisterAttached<AnimatedText, TextBlock, AnimationType>("Animation", defaultValue: AnimationType.Lines);

        public static readonly AttachedProperty<int> DelayProperty =
            AvaloniaProperty.RegisterAttached<AnimatedText, TextBlock, int>("Delay", defaultValue: 100);

        static AnimatedText()
        {
            IsAnimatingProperty.Changed.AddClassHandler<TextBlock>((tb, args) =>
            {
                if (args.NewValue is true)
                {
                    _ = StartSpinner(tb);
                }
            });
        }

        public static void SetIsAnimating(TextBlock element, bool value) =>
            element.SetValue(IsAnimatingProperty, value);

        public static bool GetIsAnimating(TextBlock element) =>
            element.GetValue(IsAnimatingProperty);

        public static void SetAnimation(TextBlock element, AnimationType value) =>
            element.SetValue(AnimationProperty, value);

        public static AnimationType GetAnimation(TextBlock element) =>
            element.GetValue(AnimationProperty);

        public static void SetDelay(TextBlock element, int value) =>
            element.SetValue(DelayProperty, value);

        public static int GetDelay(TextBlock element) =>
            element.GetValue(DelayProperty);

        public static void SetFrames(TextBlock element, object value) =>
            element.SetValue(FramesProperty, value);

        public static object? GetFrames(TextBlock element) =>
            element.GetValue(FramesProperty);

        private static async Task StartSpinner(TextBlock element)
        {
            string[] animation;

            var customFrames = GetFrames(element);
            if (customFrames != null)
            {
                if (customFrames is String str)
                {
                    animation = str.Split(',');
                }
                else if (customFrames is IEnumerable<string> strEnum)
                {
                    animation = strEnum.ToArray();
                }
                else
                {
                    throw new ArgumentException("Frames must be a string or an IEnumerable<string>");
                }
            }
            else
            {
                animation = GetAnimation(element) switch
                {
                    AnimationType.Lines => Animations.Lines,
                    AnimationType.Boxes => Animations.Boxes,
                    AnimationType.QuarterBall => Animations.QuarterBalls,
                    AnimationType.HalfBalls => Animations.HalfBalls,
                    AnimationType.Balloons => Animations.Balloons,
                    AnimationType.Arcs => Animations.Arcs,
                    AnimationType.Dots => Animations.Dots,
                    AnimationType.DotDotDot => Animations.DotDotDot,
                    AnimationType.VerticalBar => Animations.VerticalBar,
                    AnimationType.HorizontalBar => Animations.HorizontalBar,
                    AnimationType.SpinArrows => Animations.SpinArrows,
                    AnimationType.Triangles => Animations.Triangles,
                    AnimationType.Wave => Animations.Wave,
                    AnimationType.Braille => Animations.Braille,
                    AnimationType.Sparkle => Animations.Sparkle,
                    AnimationType.RightArrows => Animations.RightArrows,
                    AnimationType.LeftArrows => Animations.LeftArrows,
                    AnimationType.Staves => Animations.Staves,
                    AnimationType.Pulse => Animations.Pulse,
                    _ => Animations.Arcs,
                };
            }

            var originalText = element.Text;
            var frame = 0;
            while (GetIsAnimating(element))
            {
                await Dispatcher.UIThread.InvokeAsync(() => element.Text = animation[frame++ % animation.Length]);
                await Task.Delay(GetDelay(element));
            }

            // Clear local value to restore binding
            element.ClearValue(TextBlock.TextProperty);
            if (element.Text != originalText)
            {
                await Dispatcher.UIThread.InvokeAsync(() => element.Text = originalText);
            }
        }
    }
}