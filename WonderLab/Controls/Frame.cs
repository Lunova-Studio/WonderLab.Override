using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Threading;
using System;
using System.Threading;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Media.Transitions;

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

    private async void RunAnimation(object page) {
        using (_cancellationTokenSource) {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource = new();
        }

        if (_controlType is ControlType.Control1) {
            _PART_LeftContentPresenter.Content = page;

            if (_PART_RightContentPresenter.Content is Page oldPage)
                oldPage.InvokeUnNavigated();

            if (PageTransition != null) {
                await PageTransition.Start(_PART_RightContentPresenter, _PART_LeftContentPresenter, true, _cancellationTokenSource.Token);
            } else {
                _PART_RightContentPresenter.IsVisible = false;
                _PART_LeftContentPresenter.IsVisible = true;
            }

            _controlType = ControlType.Control2;
        } else {
            _PART_RightContentPresenter.Content = page;

            if (_PART_LeftContentPresenter.Content is Page oldPage)
                oldPage.InvokeUnNavigated();

            if (PageTransition != null) {
                await PageTransition.Start(_PART_LeftContentPresenter, _PART_RightContentPresenter, false, _cancellationTokenSource.Token);
            } else {
                _PART_LeftContentPresenter.IsVisible = false;
                _PART_RightContentPresenter.IsVisible = true;
            }

            _controlType = ControlType.Control1;
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        _PART_LeftContentPresenter = e.NameScope.Find<ContentPresenter>("PART_LeftContentPresenter");
        _PART_RightContentPresenter = e.NameScope.Find<ContentPresenter>("PART_RightContentPresenter");

        _PART_LeftContentPresenter.Opacity = 0;
        _PART_RightContentPresenter.Opacity = 0;
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == PageKeyProperty 
            && PageProvider is not null 
            && !string.IsNullOrEmpty(change.GetNewValue<string>())) {
            var page = await Dispatcher.UIThread.InvokeAsync(() => PageProvider.GetPage(change.GetNewValue<string>()), DispatcherPriority.Background);
            RunAnimation(page);
        }
    }
}