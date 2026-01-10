using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using System.Diagnostics;
using WonderLab.Models;

namespace WonderLab.ViewModels.Pages.GameSetting;

public sealed partial class MinecraftGalleryPageViewModel : ObservableRecipient, IRecipient<ValueChangedMessage<MinecraftModel>> {
    [ObservableProperty] private string _title;

    public MinecraftGalleryPageViewModel() {
    }

    public void Receive(ValueChangedMessage<MinecraftModel> message) {
        Debug.WriteLine(message.Value.Title);
        Title = message.Value.Title;
    }

    protected override void OnActivated() {
        Messenger.RegisterAll(this, "ActiveMinecraftChanged");
    }
}