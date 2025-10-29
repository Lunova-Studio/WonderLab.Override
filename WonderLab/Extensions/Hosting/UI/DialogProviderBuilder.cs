using Avalonia.Controls;
using System;
using System.Collections.Generic;

namespace WonderLab.Extensions.Hosting.UI;

public abstract class DialogProviderBuilder<TDialogProvider, TDialogBase> where TDialogProvider : DialogProvider<TDialogBase> {
    protected readonly Dictionary<string, DialogDescriptor> _registeredDialogs = [];

    public IDictionary<string, DialogDescriptor> RegisteredDialogs => _registeredDialogs;

    public DialogProviderBuilder() { }

    public DialogProviderBuilder<TDialogProvider, TDialogBase> AddDialog<TDialog, TViewModel>(string key)
        => AddDialog(key, typeof(TDialog), typeof(TViewModel));

    public DialogProviderBuilder<TDialogProvider, TDialogBase> AddDialog<TDialog>(string key)
        => AddDialog(key, typeof(TDialog));

    public DialogProviderBuilder<TDialogProvider, TDialogBase> AddDialog(string key, Type dialogType, Type viewModelType = null) {
        _registeredDialogs.Add(key, new DialogDescriptor(dialogType, viewModelType));
        return this;
    }

    public abstract TDialogProvider Build(IServiceProvider serviceProvider);
}

public class AvaloniaDialogProviderBuilder : DialogProviderBuilder<AvaloniaDialogProvider, UserControl> {
    public new Dictionary<string, DialogDescriptor> RegisteredDialogs => _registeredDialogs;

    public override AvaloniaDialogProvider Build(IServiceProvider serviceProvider) =>
        new(_registeredDialogs, serviceProvider);
}
