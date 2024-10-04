using Avalonia;
using Avalonia.Controls;
using System.Linq;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using WonderLab.Extensions;
using Avalonia.Controls.Primitives;
using System.Threading.Tasks;
using System;
using System.Threading;
using Avalonia.Threading;
using Avalonia.Interactivity;

namespace WonderLab.Views.Controls;

public sealed class PageSwitcher : SelectingItemsControl {
    private Button _gobackButton;
    private Button _goforwardButton;
    private TextBlock _numericalIndicator;

    private CancellationTokenSource _cancellationTokenSource;
    private FrozenDictionary<int, ObservableCollection<object>> _cachePageData;

    public static readonly StyledProperty<int> PageIndexProperty =
        AvaloniaProperty.Register<PageSwitcher, int>(nameof(PageIndex), 1);

    public static readonly StyledProperty<int> MaxPageItemCountProperty =
        AvaloniaProperty.Register<PageSwitcher, int>(nameof(MaxPageItemCount), 15);

    public static readonly StyledProperty<ObservableCollection<object>> SelectPageItemsProperty =
        AvaloniaProperty.Register<PageSwitcher, ObservableCollection<object>>(nameof(SelectPageItems));

    public int PageIndex {
        get => GetValue(PageIndexProperty);
        set => SetValue(PageIndexProperty, value);
    }

    public int MaxPageItemCount {
        get => GetValue(MaxPageItemCountProperty);
        set => SetValue(MaxPageItemCountProperty, value);
    }

    public ObservableCollection<object> SelectPageItems {
        get => GetValue(SelectPageItemsProperty);
        set => SetValue(SelectPageItemsProperty, value);
    }

    private void Split() {
        var dict = ((ItemsSource ?? Items) as IEnumerable<object>)
            .Select((value, index) => new { Index = index, Value = value })
            .GroupBy(x => x.Index / MaxPageItemCount)
            .ToDictionary(group => group.Key, group => group.Select(v => v.Value).ToObservableList());

        _cachePageData = dict?.ToFrozenDictionary();

        if (_cachePageData.TryGetValue(PageIndex - 1, out var items)) {
            SelectPageItems.Load(items);

            if (_numericalIndicator is null) {
                return;
            }

            _numericalIndicator.Text = $"{PageIndex}/{_cachePageData.Count}";
        }
    }

    private void Goback() {
        if (PageIndex - 1 is 0) {
            return;
        }

        PageIndex--;
        if (_cachePageData.TryGetValue(PageIndex -1, out var items)) {
            SelectPageItems.Load(items);
            _numericalIndicator.Text = $"{PageIndex}/{_cachePageData.Count}";
        }
    }

    private void Goforward() {
        if (PageIndex + 1 > _cachePageData.Count) {
            return;
        }

        PageIndex++;
        if (_cachePageData.TryGetValue(PageIndex - 1, out var items)) {
            SelectPageItems.Load(items);
            _numericalIndicator.Text = $"{PageIndex}/{_cachePageData.Count}";
        }
    }

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
        base.OnApplyTemplate(e);

        SelectPageItems = [];

        _gobackButton = e.NameScope.Find<Button>("PART_GobackButton");
        _goforwardButton = e.NameScope.Find<Button>("PART_GoforwardButton");
        _numericalIndicator = e.NameScope.Find<TextBlock>("PART_NumericalIndicator");

        _gobackButton.Click += (_, _) => Goback();
        _goforwardButton.Click += (_, _) => Goforward();
    }

    protected override async void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {
        base.OnPropertyChanged(change);

        if (change.Property == ItemsSourceProperty && change.NewValue != null) {
            Split();
        }

        if (change.Property == PageIndexProperty) {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = new();

            _numericalIndicator.FontSize = 18;
            await Task.Delay(TimeSpan.FromSeconds(0.2d), _cancellationTokenSource.Token).ContinueWith(x => {
                if (x.IsCompletedSuccessfully) {
                    Dispatcher.UIThread.Post(() => _numericalIndicator.FontSize = 14, DispatcherPriority.Render);
                }
            });
        }
    }
}