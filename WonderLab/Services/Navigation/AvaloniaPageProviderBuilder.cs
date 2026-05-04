using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace WonderLab.Services.Navigation;

public sealed class AvaloniaPageProviderBuilder {
    private readonly Dictionary<string, PageDescriptor> _pages = [];

    public IReadOnlyDictionary<string, PageDescriptor> RegisteredPages => _pages;

    public void Register<TPage>()
        where TPage : UserControl {
        var key = typeof(TPage).FullName!;
        _pages[key] = new PageDescriptor(typeof(TPage), null);
    }

    public void Register<TPage, TViewModel>()
        where TPage : UserControl
        where TViewModel : class {
        var key = typeof(TViewModel).FullName!;
        _pages[key] = new PageDescriptor(typeof(TPage), typeof(TViewModel));
    }

    public AvaloniaPageProvider Build(IServiceProvider provider)
        => new(_pages, provider);
}