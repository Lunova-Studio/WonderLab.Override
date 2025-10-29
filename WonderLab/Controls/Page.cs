using Avalonia.Controls;
using System;

namespace WonderLab.Controls;

public class Page : UserControl {
    protected override Type StyleKeyOverride => typeof(Page);

    public event EventHandler<EventArgs> Navigated;
    public event EventHandler<EventArgs> UnNavigated;

    public void InvokeNavigated() => Navigated?.Invoke(this, EventArgs.Empty);
    public void InvokeUnNavigated() => UnNavigated?.Invoke(this, EventArgs.Empty);
}