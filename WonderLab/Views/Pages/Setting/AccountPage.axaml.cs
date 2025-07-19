using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using MinecraftLaunch.Base.Models.Authentication;
using WonderLab.Classes.Models.Messaging;
using WonderLab.Controls;
using WonderLab.Services.Authentication;

namespace WonderLab;

public partial class AccountPage : Page {
    public AccountPage() {
        InitializeComponent();
    }
}