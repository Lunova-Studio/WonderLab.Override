using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Threading;
using WonderLab.Controls.Media.Transitions;
using WonderLab.Extensions.Hosting.UI;

namespace WonderLab.Controls;

public sealed class Frame : TemplatedControl {
    enum ControlType {
        Control1,
        Control2
    }

    private ControlType _controlType;
    private ContentPresenter _PART_LeftContentPresenter;
    private ContentPresenter _PART_RightContentPresenter;
    private CancellationTokenSource _cancellationTokenSource = new();

    public static readonly StyledProperty<AvaloniaPageProvider> PageProviderProperty =
        AvaloniaProperty.Register<Frame, AvaloniaPageProvider>(nameof(PageProvider), default);

    public static readonly StyledProperty<string> PageKeyProperty =
        AvaloniaProperty.Register<Frame, string>(nameof(PageKey));

    public static readonly StyledProperty<string> DefaultPageKeyProperty =
        AvaloniaProperty.Register<Frame, string>(nameof(DefaultPageKey), "Home");

    public static readonly StyledProperty<IPageTransition> PageTransitionProperty =
        AvaloniaProperty.Register<Frame, IPageTransition>(nameof(PageTransition), new DefaultPageTransition(TimeSpan.FromMilliseconds(500)));

    public string PageKey {
        get => GetValue(PageKeyProperty);
        set => SetValue(PageKeyProperty, value);
    }

    public string DefaultPageKey {
        get => GetValue(DefaultPageKeyProperty);
        set => SetValue(DefaultPageKeyProperty, value);
    }

    public IPageTransition PageTransition {
        get => GetValue(PageTransitionProperty);
        set => SetValue(PageTransitionProperty, value);
    }

    public AvaloniaPageProvider PageProvider {
        get => GetValue(PageProviderProperty);
        set => SetValue(PageProviderProperty, value);
    }

    private void RunAnimation(object page) {
        using (_cancellationTokenSource) {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new();
        }

        Dispatcher.UIThread.Post(async () => {
            if (_controlType is ControlType.Control1) {
                _PART_LeftContentPresenter.Content = page;
                await PageTransition.Start(_PART_RightContentPresenter, _PART_LeftContentPresenter, true, _cancellationTokenSource.Token);
                _controlType = ControlType.Control2;
            } else {
                _PART_RightContentPresenter.Content = page;
                await PageTransition.Start(_PART_LeftContentPresenter, _PART_RightContentPresenter, false, _cancellationTokenSource.Token);
                _controlType = ControlType.Control1;
            }
        }, DispatcherPriority.Render);
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_LeftContentPresenter = e.NameScope.Find<ContentPresenter>("PART_LeftContentPresenter");
        _PART_RightContentPresenter = e.NameScope.Find<ContentPresenter>("PART_RightContentPresenter");
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == PageKeyProperty && PageProvider is not null) {
            var page = await Dispatcher.UIThread.InvokeAsync(() => PageProvider.GetPage(change.GetNewValue<string>()), DispatcherPriority.Background);
            RunAnimation(page);
        }
    }
}