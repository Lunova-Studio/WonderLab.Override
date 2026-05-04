using Avalonia.Animation.Easings;
using System;
using System.Collections.Generic;
using System.Text;

namespace WonderLab.UI.Easings;

public sealed class MaterialBackEaseIn : Easing {
    public Amplitude Amplitude { get; set; } = Amplitude.Middle;

    public override double Ease(double progress) {
        double c1 = Amplitude switch {
            Amplitude.Weak => 0.9,
            Amplitude.Middle => 1.15,
            Amplitude.Strong => 1.5,
            Amplitude.Max => 1.8,
            _ => 0.8
        };

        double c2 = c1 * 1.525;
        if (progress < 0.5) {
            double term = 2 * progress;
            return (Math.Pow(term, 2) * ((c2 + 1) * term - c2)) / 2.0;
        } else {
            double term = 2 * progress - 2;
            return (Math.Pow(term, 2) * ((c2 + 1) * term + c2) + 2) / 2.0;
        }
    }
}

public sealed class MaterialBackEaseOut : Easing {
    public Amplitude Amplitude { get; set; } = Amplitude.Strong;

    public override double Ease(double progress) {
        double c1 = Amplitude switch {
            Amplitude.Weak => 0.9,
            Amplitude.Middle => 1.15,
            Amplitude.Strong => 1.5,
            Amplitude.Max => 1.8,
            _ => 0.8
        };

        double c3 = c1 + 1;
        return 1 + c3 * Math.Pow(progress - 1, 3) + c1 * Math.Pow(progress - 1, 2);
    }
}

public sealed class MaterialBackEaseInOut : Easing {
    public Amplitude Amplitude { get; set; } = Amplitude.Middle;

    public override double Ease(double progress) {
        double c1 = Amplitude switch {
            Amplitude.Weak => 0.9,
            Amplitude.Middle => 1.15,
            Amplitude.Strong => 1.5,
            Amplitude.Max => 1.8,
            _ => 0.8
        };

        double c3 = c1 + 1;
        double t = progress;
        double smoothedStart = t * t * (2 - t);
        double p = smoothedStart;

        return 1 + c3 * Math.Pow(p - 1, 3) + c1 * Math.Pow(p - 1, 2);
    }
}

public enum Amplitude {
    Weak,
    Middle,
    Strong,
    Max
}