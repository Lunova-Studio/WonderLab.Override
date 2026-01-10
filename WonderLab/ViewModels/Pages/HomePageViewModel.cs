using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using WonderLab.Models;
using WonderLab.Services.Game;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : ObservableRecipient {
    private MinecraftService _minecraftService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WeekPlayTime))]
    [NotifyPropertyChangedFor(nameof(TotalPlayTime))]
    [NotifyPropertyChangedFor(nameof(TodayPlayTime))]
    [NotifyPropertyChangedFor(nameof(TotalMinecraftCount))]
    private ObservableCollection<MinecraftModel> _minecrafts = [];

    public int TotalMinecraftCount => Minecrafts.Count;
    public TimeSpan TotalPlayTime => GetTotalPlayTime(Minecrafts);
    public TimeSpan TodayPlayTime => GetTodayPlayTime(Minecrafts);
    public TimeSpan WeekPlayTime => GetThisWeekPlayTime(Minecrafts);

    public ObservableCollection<string> News { get; } = [
        "1145/14/19",
        "1145/14/19",
        "1145/14/19",
        "1145/14/19",
        "1145/14/19",
        "1145/14/19",
        "1145/14/19",
        ];

    public HomePageViewModel(MinecraftService minecraftService) {
        _minecraftService = minecraftService;
        OnMinecraftFolderChanged();
    }

    protected override void OnActivated() {
        _minecraftService.MinecraftFolderChanged += OnMinecraftFolderChanged;
    }

    protected override void OnDeactivated() {
        _minecraftService.MinecraftFolderChanged -= OnMinecraftFolderChanged;
    }

    private void OnMinecraftFolderChanged() {
        Minecrafts = new(_minecraftService.Minecrafts.SelectMany(x => x.Favorites));
    }

    public static TimeSpan GetTotalPlayTime(IEnumerable<MinecraftModel> games) {
        return TimeSpan.FromSeconds(games.SelectMany(g => g.Sessions).Sum(s => s.Duration.TotalSeconds));
    }

    public static TimeSpan GetTodayPlayTime(IEnumerable<MinecraftModel> games) {
        DateTime today = DateTime.Today;
        DateTime tomorrow = today.AddDays(1);

        return TimeSpan.FromSeconds(games.SelectMany(g => g.Sessions).Where(s => s.StartTime >= today && s.StartTime < tomorrow)
                 .Sum(s => s.Duration.TotalSeconds));
    }

    public static TimeSpan GetThisWeekPlayTime(IEnumerable<MinecraftModel> games) {
        var now = DateTime.Now;
        int currentWeek = ISOWeek.GetWeekOfYear(now);
        int currentYear = now.Year;

        return TimeSpan.FromSeconds(
            games.SelectMany(g => g.Sessions)
                 .Where(s => ISOWeek.GetWeekOfYear(s.StartTime) == currentWeek && s.StartTime.Year == currentYear)
                 .Sum(s => s.Duration.TotalSeconds)
        );
    }
}