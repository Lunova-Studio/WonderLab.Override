using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Utilities;

namespace WonderLab.Controls;

[StyledProperty(typeof(string), "PageKey")]
[StyledProperty(typeof(object), "Content")]
[StyledProperty(typeof(string), "DefaultPageKey")]
[StyledProperty(typeof(IFrameTransition), "FrameTransition")]
public sealed partial class Frame : TemplatedControl {
    private ContentPresenter _PART_ContentPresenter;
    private CancellationTokenSource _cancellationTokenSource = new();

    private readonly Stack<string> _backPages = new();
    private readonly Stack<string> _forwardPages = new();

    public bool CanGoBack => _backPages.Count != 0;
    public bool CanGoForward => _forwardPages.Count != 0;

    public static AvaloniaPageProvider PageProvider { get; set; }

    public event Action Navigated;

    protected override Type StyleKeyOverride => typeof(Frame);

    public void Navigate(string key) {
        if (!string.IsNullOrWhiteSpace(PageKey))
            _backPages.Push(PageKey);

        PageKey = key;
        _forwardPages.Clear();
    }

    public void GoBack() {
        if (_backPages.Count != 0) {
            _forwardPages.Push(PageKey);
            PageKey = _backPages.Pop();
        }
    }

    public void GoForward() {
        if (_forwardPages.Count != 0) {
            _backPages.Push(PageKey);
            PageKey = _forwardPages.Pop();
        }
    }

    private async void RunAnimation(object newPage) {
        try {
            using (_cancellationTokenSource) {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = new();
            }

            if ((newPage as UserControl).DataContext is ObservableRecipient recipient)
                recipient.IsActive = true;

            if (FrameTransition is null) {
                _PART_ContentPresenter.Content = newPage;
            } else {
                if(_PART_ContentPresenter.Content is not null)
                    await FrameTransition.Animate(_PART_ContentPresenter, true, _cancellationTokenSource.Token);
                _PART_ContentPresenter.Content = newPage;
                await FrameTransition.Animate(_PART_ContentPresenter, false, _cancellationTokenSource.Token);
            }
        } finally {
        }
    }

    protected override async void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        FrameTransition ??= new DefaultFrameTransition(TimeSpan.FromMilliseconds(500));
        await Task.Delay(50);
        PageKey = DefaultPageKey;
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);
        _PART_ContentPresenter = e.NameScope.Find<ContentPresenter>("PART_ContentPresenter");
        _PART_ContentPresenter.Opacity = 0;
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == PageKeyProperty
            && PageProvider is not null
            && !string.IsNullOrEmpty(change.GetNewValue<string>())) {
            var page = await Dispatcher.UIThread.InvokeAsync(() => PageProvider.GetPage(change.GetNewValue<string>()), DispatcherPriority.Background);
            RunAnimation(page);
            Navigated?.Invoke();
        }
    }
}

public interface IFrameTransition {
    Task Animate(Animatable animatable, bool isForward, CancellationToken cancellationToken);
    Task ClearVisualTree(TimeSpan startTime, Action action, CancellationToken cancellationToken);
}

public sealed class DefaultFrameTransition : IFrameTransition {
    public TimeSpan Duration { get; set; }
    public Easing Easing { get; set; } = new ExponentialEaseOut();

    public DefaultFrameTransition() { }

    public DefaultFrameTransition(TimeSpan duration) {
        Duration = duration;
    }

    public async Task ClearVisualTree(TimeSpan startTime, Action action, CancellationToken cancellationToken) {
        try {
            await Task.Delay(startTime, cancellationToken);
        } catch (Exception) {}

        action();
    }

    public async Task Animate(Animatable animatable, bool isForward, CancellationToken cancellationToken) {
        var control = animatable as ContentPresenter;
        var tasks = new List<Task>();
        var controlEV = ElementComposition.GetElementVisual(control);
        var group = controlEV.Compositor.CreateAnimationGroup();
        var size = controlEV!.Size;

        controlEV!.CenterPoint = new Vector3((float)size.X / 2, (float)size.Y / 2, (float)controlEV.CenterPoint.Z);
        if (isForward) {
            var opacityAni = CompositionAnimationUtil.CreateScalarAnimation(controlEV, 1, 0, TimeSpan.FromMilliseconds(400), Easing);
            var scaleAni = CompositionAnimationUtil.CreateVector3Animation(controlEV, new(1), new(0.9f), Duration, Easing);

            scaleAni.Target = CompositionAnimationUtil.PROPERTY_SCALE;
            opacityAni.Target = CompositionAnimationUtil.PROPERTY_OPACITY;

            group.Add(scaleAni);
            group.Add(opacityAni);

            tasks.Add(ClearVisualTree(Duration / 2, () => control.Content = null, cancellationToken));
        } else {
            var opacityAni = CompositionAnimationUtil.CreateScalarAnimation(controlEV, 0, 1, Duration, Easing);
            var scaleAni = CompositionAnimationUtil.CreateVector3Animation(controlEV, new(1.2f), new(1), Duration, Easing);

            scaleAni.Target = CompositionAnimationUtil.PROPERTY_SCALE;
            opacityAni.Target = CompositionAnimationUtil.PROPERTY_OPACITY;

            group.Add(scaleAni);
            group.Add(opacityAni);
        }

        tasks.Add(Dispatcher.UIThread.InvokeAsync(async () => controlEV.StartAnimationGroup(group)));

        await Task.WhenAll(tasks);
    }
}