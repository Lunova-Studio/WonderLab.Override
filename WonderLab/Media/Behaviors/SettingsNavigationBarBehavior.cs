using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.VisualTree;
using Avalonia.Xaml.Interactivity;
using System;
using System.Threading;
using System.Threading.Tasks;
using WonderLab.Classes.Enums;
using WonderLab.Extensions;
using WonderLab.Media.Easings;

namespace WonderLab.Media.Behaviors;

[Obsolete("过于石山等待后续优化再投入使用")]
[StyledProperty(typeof(Border), "BackwardControl")]
[StyledProperty(typeof(ListBox), "NavigationControl")]
public sealed partial class SettingsNavigationBarBehavior : Behavior<Control> {
    private bool _isHidden = false;
    private TopLevel _visualRoot = null;
    private Button _backwardButton = null;
    private CancellationTokenSource _cancellationTokenSource = new();

    protected override void OnLoaded() {
        base.OnLoaded();

        if (_visualRoot.Bounds.Width < 839) {
            NavigationControl.Classes.Add("max");
            NavigationControl.Opacity = 1;
            NavigationControl.ZIndex = 2;
            NavigationControl.SelectionChanged += OnNavigationControlSelectionChanged;

            Grid.SetColumn(NavigationControl, 1);
        } else {
            NavigationControl.Classes.Add("normal");
            NavigationControl.SelectedIndex = 0;
        }
    }

    protected override void OnAttachedToVisualTree() {
        base.OnAttachedToVisualTree();

        if (AssociatedObject is null)
            return;

        _visualRoot = AssociatedObject.GetVisualRoot() as TopLevel;
        _visualRoot.SizeChanged += OnSizeChanged;

        _backwardButton = BackwardControl.FindControl<Button>("PART_BackwardButton");
        _backwardButton.Click += OnBackwardButtonClick;
    }

    protected override void OnDetachedFromVisualTree() {
        base.OnDetachedFromVisualTree();

        _visualRoot.SizeChanged -= OnSizeChanged;
        _backwardButton.Click -= OnBackwardButtonClick;
        NavigationControl.SelectionChanged -= OnNavigationControlSelectionChanged;
    }

    private void Refresh() {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new();
    }

    private async Task HiddenControlAsync() {
        _isHidden = true;

        Refresh();

        NavigationControl.Opacity = 0;
        await NavigationControl.Animate(Layoutable.WidthProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(NavigationControl.Width)
            .To(0)
            .RunAsync(_cancellationTokenSource.Token);

        ChangeMaxClasses();
    }

    private async Task ExpandControlAsync() {
        _isHidden = false;

        Refresh();
        ChangeNormalClasses();

        AssociatedObject.Opacity = 1;
        NavigationControl.Opacity = 1;
        await NavigationControl.Animate(Layoutable.WidthProperty)
            .WithEasing(new WonderBackEaseOut() { Amplitude = Amplitude.Weak })
            .WithDuration(TimeSpan.FromMilliseconds(300))
            .From(NavigationControl.Width)
            .To(195)
            .RunAsync(_cancellationTokenSource.Token);
    }

    private void ChangeMaxClasses() {
        _ = BackwardControl.Animate(Layoutable.HeightProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(BackwardControl.Height)
            .To(54)
            .RunAsync(_cancellationTokenSource.Token);

        _ = BackwardControl.Animate(Visual.OpacityProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(BackwardControl.Opacity)
            .To(1)
            .RunAsync(_cancellationTokenSource.Token);

        NavigationControl.Classes.Clear();
        NavigationControl.Classes.Add("max");

        Grid.SetColumn(NavigationControl, 1);
        NavigationControl.Width = double.NaN;
        NavigationControl.SelectionChanged += OnNavigationControlSelectionChanged;
    }

    private void ChangeNormalClasses() {
        NavigationControl.Classes.Clear();
        NavigationControl.Classes.Add("normal");

        Grid.SetColumn(NavigationControl, 0);
        NavigationControl.Width = 0;
        NavigationControl.SelectionChanged -= OnNavigationControlSelectionChanged;



        _ = BackwardControl.Animate(Layoutable.HeightProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(BackwardControl.Height)
            .To(0)
            .RunAsync(_cancellationTokenSource.Token);

        _ = BackwardControl.Animate(Visual.OpacityProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(BackwardControl.Opacity)
            .To(0)
            .RunAsync(_cancellationTokenSource.Token);
    }

    private async void OnBackwardButtonClick(object sender, RoutedEventArgs e) {
        _ = BackwardControl.Animate(Layoutable.HeightProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(BackwardControl.Height)
            .To(0)
            .RunAsync(_cancellationTokenSource.Token);

        _ = BackwardControl.Animate(Visual.OpacityProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(BackwardControl.Opacity)
            .To(0)
            .RunAsync(_cancellationTokenSource.Token);

        NavigationControl.ZIndex = 2;
        NavigationControl.SelectedIndex = -1;

        await AssociatedObject.Animate(Visual.OpacityProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(AssociatedObject.Opacity)
            .To(0)
            .RunAsync(_cancellationTokenSource.Token);

        await NavigationControl.Animate(Visual.OpacityProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(NavigationControl.Opacity)
            .To(1)
            .RunAsync(_cancellationTokenSource.Token);
    }

    private async void OnSizeChanged(object sender, SizeChangedEventArgs e) {
        if (!e.WidthChanged)
            return;

        await Dispatcher.UIThread.InvokeAsync(async () => {
            if (e.NewSize.Width > 839 && _isHidden) {
                if (NavigationControl.SelectedIndex is -1)
                    NavigationControl.SelectedIndex = 0;

                await ExpandControlAsync();
            } else if (e.NewSize.Width < 839 && !_isHidden) {
                await HiddenControlAsync();
            }
        });
    }

    private async void OnNavigationControlSelectionChanged(object sender, SelectionChangedEventArgs e) {
        if (NavigationControl.SelectedIndex is -1)
            return;

        NavigationControl.ZIndex = 0;

        await NavigationControl.Animate(Visual.OpacityProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(NavigationControl.Opacity)
            .To(0)
            .RunAsync(_cancellationTokenSource.Token);


        await AssociatedObject.Animate(Visual.OpacityProperty)
            .WithEasing(new CubicEaseOut())
            .WithDuration(TimeSpan.FromMilliseconds(200))
            .From(NavigationControl.Opacity)
            .To(1)
            .RunAsync(_cancellationTokenSource.Token);

        BackwardControl.IsVisible = true;
        BackwardControl.Opacity = 1;
    }
}