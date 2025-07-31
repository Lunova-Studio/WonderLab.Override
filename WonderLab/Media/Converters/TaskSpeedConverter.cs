using Avalonia.Data.Converters;
using MinecraftLaunch.Components.Downloader;
using System;
using System.Globalization;
using WonderLab.Classes.Models;

namespace WonderLab.Media.Converters;

public sealed class TaskSpeedConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is TaskModel task)
            return $"{(task.TaskJob.IsSupportSpeed
                ? DefaultDownloader.FormatSize(task.TaskJob.Speed.Value, true) : "")} {task.TaskJob.RunningTimeText}";

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}