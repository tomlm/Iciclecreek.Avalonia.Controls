﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;

namespace Iciclecreek.Avalonia.Controls
{

    public class TextBlockSpinner : TextBlock
    {
        public static readonly StyledProperty<int> SpeedProperty =
            AvaloniaProperty.Register<TextBlockSpinner, int>(nameof(Speed), defaultValue: 100);

        public static readonly StyledProperty<bool> IsActiveProperty =
            AvaloniaProperty.Register<TextBlockSpinner, bool>(nameof(IsActive), defaultValue: false);

        public static readonly StyledProperty<AnimationType> AnimationTypeProperty =
            AvaloniaProperty.Register<TextBlockSpinner, AnimationType>(nameof(AnimationType), defaultValue: AnimationType.Arcs);

        private CancellationTokenSource _cancelationTokenSource;
        private int _frame;
        private string[] _animation;
        private Task _spinnerTask;

        public TextBlockSpinner()
        {
        }

        protected async override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            switch (change.Property.Name)
            {
                case nameof(IsActive):
                    if (IsActive)
                    {
                        await StopSpinner();

                        _spinnerTask = StartSpinner();
                        this.IsVisible = true;
                    }
                    else
                    {
                        await StopSpinner();
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

        public AnimationType AnimationType
        {
            get { return GetValue(AnimationTypeProperty); }
            set { SetValue(AnimationTypeProperty, value); }
        }

        private async Task StartSpinner()
        {
            StopSpinner();

            LoadAnimation();

            _cancelationTokenSource = new CancellationTokenSource();
            while (_cancelationTokenSource.IsCancellationRequested == false)
            {
                await Dispatcher.UIThread.InvokeAsync(() => Text = _animation[_frame++ % _animation.Length]);
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
            _animation = AnimationType switch
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
            _frame = 0;
        }

        private async Task StopSpinner()
        {
            if (_cancelationTokenSource != null)
            {
                _cancelationTokenSource.Cancel();

                if (_spinnerTask != null)
                {
                    await _spinnerTask;

                    _spinnerTask = null;
                }
            }
        }
    }
}