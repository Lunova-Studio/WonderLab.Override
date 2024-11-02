using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using System.Collections.Generic;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace WonderLab.Controls.Media.Transitions;

public sealed class EntrancePageTransition : IPageTransition {
    public TimeSpan Duration { get; set; }
    public Easing Easing { get; set; } = new CircularEaseInOut();

    public EntrancePageTransition() { }

    public EntrancePageTransition(TimeSpan duration = default) {
        Duration = duration;
    }

    public async Task Start(Visual from, Visual to, bool forward, CancellationToken cancellationToken) {
        if (cancellationToken.IsCancellationRequested) {
            return;
        }

        var tasks = new List<Task>();

        if (from is not null) {
            var animation = new Animation {
                Easing = Easing,
                Duration = Duration,
                FillMode = FillMode.Forward,
                Children = {
                    new KeyFrame {
                        Cue = new(0d),
                        Setters = {
                            new Setter {
                                Value = 1d,
                                Property = Visual.OpacityProperty
                            },
                        }
                    },

                    new KeyFrame {
                        Cue= new(1d),
                        Setters = {
                            new Setter {
                                Value = 0d,
                                Property = Visual.OpacityProperty
                            },
                        }
                    }
                }
            };

            tasks.Add(animation.RunAsync(from, cancellationToken));
        }

        if (to is not null) {
            var animation = new Animation {
                Easing = Easing,
                Duration = Duration,
                FillMode = FillMode.Forward,
                Children = {
                    new KeyFrame {
                        Cue = new(0d),
                        Setters = {
                            new Setter {
                                Value = 0d,
                                Property = Visual.OpacityProperty
                            },
                            new Setter {
                                Value = 130d,
                                Property = TranslateTransform.YProperty
                            },
                        }
                    },

                    new KeyFrame {
                        Cue= new(1d),
                        Setters = {
                            new Setter {
                                Value = 1d,
                                Property = Visual.OpacityProperty
                            },
                            new Setter {
                                Value = 0d,
                                Property = TranslateTransform.YProperty
                            },
                        }
                    }
                }
            };

            tasks.Add(animation.RunAsync(to, cancellationToken));
        }

        await Task.WhenAll(tasks);

        (from as Control).IsHitTestVisible = false;
        (to as Control).IsHitTestVisible = true;
    }

    private static Visual GetVisualParent(Visual from, Visual to) {
        Visual p1 = (from ?? to)!.GetVisualParent(),
            p2 = (to ?? from)!.GetVisualParent();

        if (p1 != null && p2 != null && p1 != p2) {
            throw new ArgumentException("Controls for Very must have same parent.");
        }

        return p1 ?? throw new InvalidOperationException("Cannot determine visual parent.");
    }
}