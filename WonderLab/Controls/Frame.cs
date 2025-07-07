using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Threading;
using System;
using System.Threading;
using WonderLab.Extensions.Hosting.UI;
using WonderLab.Media.Transitions;
using WonderLab.SourceGenerator.Attributes;

namespace WonderLab.Controls;

[StyledProperty(typeof(string), "PageKey")]
[StyledProperty(typeof(string), "DefaultPageKey", "Home")]
[StyledProperty(typeof(IPageTransition), "PageTransition")]
[StyledProperty(typeof(AvaloniaPageProvider), "PageProvider")]
public sealed partial class Frame : TemplatedControl {
    enum ControlType {
        Control1,
        Control2
    }

    private ControlType _controlType;
    private ContentPresenter _PART_LeftContentPresenter;
    private ContentPresenter _PART_RightContentPresenter;
    private CancellationTokenSource _cancellationTokenSource = new();

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

    protected override void OnLoaded(RoutedEventArgs e) {
        base.OnLoaded(e);
        PageTransition ??= new DefaultPageTransition(TimeSpan.FromMilliseconds(500));
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