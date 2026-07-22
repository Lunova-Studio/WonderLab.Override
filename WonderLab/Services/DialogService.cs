using System;
using System.Threading.Tasks;
using Avalonia.Animation.Easings;
using DialogHostAvalonia;
using Microsoft.Extensions.Logging;
using WonderLab.Services.Navigation;
using ZLogger;

namespace WonderLab.Services;

public sealed class DialogService {
    private const string PART_DialogHost = "PART_DialogHost";
    
    private readonly AvaloniaPageProvider _provider;
    private readonly ILogger<DialogService> _logger;
    
    public DialogService(AvaloniaPageProvider provider, ILogger<DialogService> logger) {
        _logger = logger;
        _provider = provider;
    }

    public Task ShowDialogAsync() {
        return DialogHost.IsDialogOpen(PART_DialogHost) 
            ? throw new InvalidOperationException("DialogHost can't be opened")
            : DialogHost.Show("Blur can be controlled via BlurBackground and BlurBackgroundRadius properties.",
                PART_DialogHost);
    }
    
    public Task<object> ShowDialogAsync<TViewModel>() where TViewModel : class {
        var key = typeof(TViewModel).FullName!;
        var content = _provider.GetPage(key);
        
#if DEBUG
        _logger.ZLogInformation($"Current key is {key}");
#endif
        
        return DialogHost.IsDialogOpen(PART_DialogHost) 
            ? throw new InvalidOperationException("DialogHost can't be opened")
            : DialogHost.Show(content, PART_DialogHost);
    }

    public void Close(object parameter) {
#if DEBUG
        _logger.ZLogInformation($"{parameter}");
#endif
        
        if(DialogHost.IsDialogOpen(PART_DialogHost))
            DialogHost.Close(PART_DialogHost, parameter);
        else 
            throw new InvalidOperationException("DialogHost can't be closed");
    }
}