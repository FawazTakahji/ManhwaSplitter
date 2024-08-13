using System;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging;
using ManhwaSplitter.Core.Services;
using ManhwaSplitter.Mobile.Messages;
using AvaloniaNotificationType = Avalonia.Controls.Notifications.NotificationType;
using NotificationType = ManhwaSplitter.Core.Enums.NotificationType;

namespace ManhwaSplitter.Mobile.Services;

public class AppNotificationService : IAppNotificationService
{
    public void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null)
    {
        AvaloniaNotificationType notificationType = type switch
        {
            NotificationType.Information => AvaloniaNotificationType.Information,
            NotificationType.Success => AvaloniaNotificationType.Success,
            NotificationType.Warning => AvaloniaNotificationType.Warning,
            NotificationType.Error => AvaloniaNotificationType.Error,
            _ => AvaloniaNotificationType.Information
        };

        WeakReferenceMessenger.Default.Send(new NotificationMessage(new Notification(
            title,
            message,
            notificationType,
            expiration ?? TimeSpan.FromSeconds(5))));
    }
}