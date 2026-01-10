using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WonderLab.Services;

public sealed class FileDialogService {
    private Window _window;

    public FileDialogService(Window targetWindow) {
        _window = targetWindow;
    }

    public async Task<IEnumerable<string>> OpenFilePickerAsync(IReadOnlyList<FilePickerFileType> fileTypes, string title) {
        var result = await _window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = fileTypes,
        });

        if (result.Count <= 0)
            return [];

        return result.Select(x => x.Path.LocalPath);
    }
}