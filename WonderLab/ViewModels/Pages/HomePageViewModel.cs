using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using WonderLab.Services;

namespace WonderLab.ViewModels.Pages;

public sealed partial class HomePageViewModel : PageViewModelBase {
    [ObservableProperty] private string _oneSentence;

    public List<string> OneSentences { get; } = [
        "今天也要好好挖矿哦",
        "苦力怕不会等你准备好",
        "启动器正在思考人生……请稍等",
        "如果你看到这个，说明启动器还没炸",
        "别问为什么卡顿，问问末影龙是不是醒了",
        "苦力怕刚刚路过，我们重新加载一下心情",
        "今天也要努力不掉进岩浆里哦",
        "挖矿五小时，收获一组煤和一段人生反思",
        "正在加载 ‘你以为你能跑得动’ 的材质包，请耐心等待"
    ];

    public static string TimeSlot => DateTime.Now.Hour switch {
        >= 0 and < 6 => "LateNight",   // 凌晨
        >= 6 and < 11 => "Morning",     // 上午
        >= 11 and < 14 => "Noon",        // 中午
        >= 14 and < 18 => "Afternoon",   // 下午
        _ => "Evening"// 晚上
    };

    public HomePageViewModel(SettingService settingService) {
        OneSentence = OneSentences[GetDailyTimeSlotRandom(0, OneSentences.Count)];
    }

    private static int GetDailyTimeSlotRandom(int min = 0, int max = 5) {
        var now = DateTime.Now;

        string seedSource = $"{now:yyyy-MM-dd}-{TimeSlot}";
        int seed = seedSource.GetHashCode();

        var random = new Random(seed);
        return random.Next(min, max);
    }
}