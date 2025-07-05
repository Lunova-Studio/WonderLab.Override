using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Xaml.Interactivity;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Extensions;
using WonderLab.Media.Easings;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Media.Behaviors;

[StyledProperty(typeof(string), "ToIcon")]
[StyledProperty(typeof(double), "FontSize")]
public sealed partial class AnimationIconBehavior : Behavior<Button> {
    private string _sourceIcon;
    private CancellationTokenSource _cancellationTokenSource = new();

    protected override void OnLoaded() {
        base.OnLoaded();

        if (AssociatedObject is not null) {
            AssociatedObject.Transitions?.Add(new DoubleTransition() {
                Property = TextBlock.FontSizeProperty,
                Duration = TimeSpan.FromSeconds(0.35),
                Easing = new WonderBackEaseOut() {
                    Amplitude = Amplitude.Weak
                }
            });

            _sourceIcon = AssociatedObject.Content.ToString();
            AssociatedObject.Click += OnClick;
        }
    }

    protected override void OnDetachedFromVisualTree() {
        base.OnDetachedFromVisualTree();

        AssociatedObject.Click -= OnClick;
    }

    private async void OnClick(object sender, Avalonia.Interactivity.RoutedEventArgs e) {
        using (_cancellationTokenSource) {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new();
        }

        try {
            await AssociatedObject.Animate(TextBlock.FontSizeProperty)
                .WithEasing(new ExponentialEaseIn())
                .WithDuration(TimeSpan.FromSeconds(0.2))
                .From(AssociatedObject.FontSize)
                .To(1)
                .RunAsync(_cancellationTokenSource.Token);

            AssociatedObject.Content = ToIcon;

            await AssociatedObject.Animate(TextBlock.FontSizeProperty)
                .WithDuration(TimeSpan.FromSeconds(0.3))
                .From(AssociatedObject.FontSize)
                .To(FontSize)
                .RunAsync(_cancellationTokenSource.Token);

            await Task.Delay(TimeSpan.FromSeconds(1), _cancellationTokenSource.Token);

            await AssociatedObject.Animate(Visual.OpacityProperty)
                .WithDuration(TimeSpan.FromSeconds(0.15))
                .From(AssociatedObject.Opacity)
                .To(0)
                .RunAsync(_cancellationTokenSource.Token);

            AssociatedObject.Content = _sourceIcon;

            await AssociatedObject.Animate(Visual.OpacityProperty)
                .WithDuration(TimeSpan.FromSeconds(0.2))
                .From(AssociatedObject.Opacity)
                .To(1)
                .RunAsync(_cancellationTokenSource.Token);

        } catch (Exception) { }
    }
}