using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace WonderLab.Extensions.Hosting.UI;

public abstract class PageProviderBuilder<TPageProvider, TPageBase> where TPageProvider : PageProvider<TPageBase> {
    protected readonly Dictionary<string, PageDescriptor> _registeredPages = [];

    public IDictionary<string, PageDescriptor> RegisteredPages => _registeredPages;

    public PageProviderBuilder() { }

    public PageProviderBuilder<TPageProvider, TPageBase> AddPage<TPage, TViewModel>(string key)
        => AddPage(key, typeof(TPage), typeof(TViewModel));

    public PageProviderBuilder<TPageProvider, TPageBase> AddPage<TPage>(string key)
        => AddPage(key, typeof(TPage));

    public PageProviderBuilder<TPageProvider, TPageBase> AddPage(string key, Type pageType, Type viewModelType = null) {
        _registeredPages.Add(key, new PageDescriptor(pageType, viewModelType));
        return this;
    }

    public abstract TPageProvider Build(IServiceProvider serviceProvider);
}

public class AvaloniaPageProviderBuilder : PageProviderBuilder<AvaloniaPageProvider, UserControl> {
    public new Dictionary<string, PageDescriptor> RegisteredPages => _registeredPages;

    public override AvaloniaPageProvider Build(IServiceProvider serviceProvider) {
        return new AvaloniaPageProvider(_registeredPages, serviceProvider);
    }
}