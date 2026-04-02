using Avalonia.Controls;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace WonderLab.Services.Navigation;

public abstract class PageProvider<TPage> {
    protected readonly IServiceProvider _services;
    protected readonly IReadOnlyDictionary<string, PageDescriptor> _registeredPages;

    protected PageProvider(
        IReadOnlyDictionary<string, PageDescriptor> registeredPages,
        IServiceProvider services) {
        _registeredPages = registeredPages;
        _services = services;
    }

    public TPage GetPage(string key) {
        var descriptor = _registeredPages[key];

        var page = (TPage)_services.GetRequiredService(descriptor.PageType);

        if (descriptor.ViewModelType is not null) {
            var vm = _services.GetRequiredService(descriptor.ViewModelType);
            ConfigureViewModel(page, vm);
        }

        return page;
    }

    public object GetViewModel(string key) {
        var descriptor = _registeredPages[key];
        if (descriptor.ViewModelType is null)
            return null;

        return _services.GetRequiredService(descriptor.ViewModelType);
    }

    protected abstract void ConfigureViewModel(TPage page, object viewModel);
}

public sealed class AvaloniaPageProvider(
    IReadOnlyDictionary<string, PageDescriptor> registeredPages,
    IServiceProvider services) : PageProvider<UserControl>(registeredPages, services) {

    protected override void ConfigureViewModel(UserControl page, object viewModel) {
        page.DataContext = viewModel;
    }
}