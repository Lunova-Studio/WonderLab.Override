using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace WonderLab.Media.Converters;

public sealed class MinecraftVersionsConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is not IEnumerable<string> versions || !versions.Any())
            return null;

        var majorVersions = versions
            .Where(v => !v.Any(char.IsLetter))
            .Select(Version.Parse)
            .Select(v => (v.Major, v.Minor))
            .Distinct()
            .OrderBy(v => v.Major)
            .ThenBy(v => v.Minor)
            .ToList();

        List<string> ranges = [];
        for (int i = 0; i < majorVersions.Count;) {
            int start = i, end = i;
            while (end + 1 < majorVersions.Count &&
                   majorVersions[end + 1].Major == majorVersions[end].Major &&
                   majorVersions[end + 1].Minor == majorVersions[end].Minor + 1) {
                end++;
            }

            var (startMajor, startMinor) = majorVersions[start];
            var (endMajor, endMinor) = majorVersions[end];

            if (end == majorVersions.Count - 1 && endMinor == 21)
                ranges.Add($"{startMajor}.{startMinor}+");
            else if (start == end)
                ranges.Add($"{startMajor}.{startMinor}");
            else
                ranges.Add($"{startMajor}.{startMinor}-{endMajor}.{endMinor}");

            i = end + 1;
        }

        return string.Join(", ", ranges);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        throw new NotImplementedException();
    }
}