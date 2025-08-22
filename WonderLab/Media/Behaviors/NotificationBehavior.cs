using Avalonia.Controls.Notifications;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Threading.Tasks;
using WonderLab.Classes.Models.Messaging;

namespace WonderLab.Media.Behaviors;

public sealed class NotificationBehavior : Behavior<WindowNotificationManager> {
    private static readonly TimeSpan Default = TimeSpan.FromSeconds(4d);

    protected override void OnLoaded() {
        WeakReferenceMessenger.Default.Register<NotificationMessage>(this, OnNotification);
    }

    protected override void OnUnloaded() {
        WeakReferenceMessenger.Default.Unregister<NotificationMessage>(this);
    }

    private async void OnNotification(object sender, NotificationMessage args) {
        if (AssociatedObject is null)
            return;

        await Dispatcher.UIThread.InvokeAsync(() => {
            AssociatedObject.Show(new Notification() {
                Message = args.Text,
                Expiration = Default,
                OnClick = args.OnClick,
                OnClose = args.OnClose,
                Type = args.NotificationType,
                Title = args.NotificationType.ToString(),
            });
        });
    }
}