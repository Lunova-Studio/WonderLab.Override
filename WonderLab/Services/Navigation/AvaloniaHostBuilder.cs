using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WonderLab.Interfaces.Navigation;

namespace WonderLab.Services.Navigation;

public sealed class AvaloniaHostBuilder : IHostApplicationBuilder {
    private readonly HostApplicationBuilder _hostBuilder = new();

    public ILoggingBuilder Logging => _hostBuilder.Logging;
    public IMetricsBuilder Metrics => _hostBuilder.Metrics;
    public IServiceCollection Services => _hostBuilder.Services;
    public IHostEnvironment Environment => _hostBuilder.Environment;
    public IConfigurationManager Configuration => _hostBuilder.Configuration;
    public IDictionary<object, object> Properties => ((IHostApplicationBuilder)_hostBuilder).Properties;

    public AvaloniaPageProviderBuilder PageProvider { get; } = new();

    public IHost Build() {
        RegisterDescriptors(PageProvider.RegisteredPages,
            d => d.PageType,
            d => d.ViewModelType);

        Services.AddSingleton(PageProvider.Build);
        Services.AddSingleton<INavigationService, NavigationService>();

        return _hostBuilder.Build();
    }

    private void RegisterDescriptors<TDescriptor>(
        IReadOnlyDictionary<string, TDescriptor> descriptors,
        Func<TDescriptor, Type> typeSelector,
        Func<TDescriptor, Type> viewModelSelector)
        where TDescriptor : class {
        foreach (var (_, descriptor) in descriptors) {
            var type = typeSelector(descriptor);
            if (type != null)
                Services.AddTransient(type);

            var vmType = viewModelSelector(descriptor);
            if (vmType != null)
                Services.AddTransient(vmType);
        }
    }

    void IHostApplicationBuilder.ConfigureContainer<TBuilder>(
        IServiceProviderFactory<TBuilder> factory,
        Action<TBuilder> configure)
        => _hostBuilder.ConfigureContainer(factory, configure);
}