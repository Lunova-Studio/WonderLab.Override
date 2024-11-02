using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.Metrics;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using WonderLab.Extensions.Hosting.UI;

namespace WonderLab.Extensions.Hosting;

public sealed class AvaloniaHostBuilder : IHostApplicationBuilder {
    private readonly HostApplicationBuilder _hostBuilder;

    public ILoggingBuilder Logging => _hostBuilder.Logging;
    public IMetricsBuilder Metrics => _hostBuilder.Metrics;
    public IServiceCollection Services => _hostBuilder.Services;
    public IHostEnvironment Environment => _hostBuilder.Environment;
    public IConfigurationManager Configuration => _hostBuilder.Configuration;
    public IDictionary<object, object> Properties => (_hostBuilder as IHostApplicationBuilder).Properties;

    public AvaloniaPageProviderBuilder PageProvider { get; private set; } = new();

    public AvaloniaHostBuilder() {
        _hostBuilder = new HostApplicationBuilder();
    }

    public IHost Build() {
        foreach (var (key, descriptor) in PageProvider.RegisteredPages) {
            Services.AddTransient(descriptor.PageType);
            
            if (descriptor.ViewModelType is not null) {
                Services.AddTransient(descriptor.ViewModelType);
            }
        }

        Services.AddSingleton(PageProvider.Build);

        return _hostBuilder.Build();
    }

    void IHostApplicationBuilder.ConfigureContainer<TBuilder>(IServiceProviderFactory<TBuilder> factory, Action<TBuilder> configure) {
        _hostBuilder.ConfigureContainer(factory, configure);
    }
}