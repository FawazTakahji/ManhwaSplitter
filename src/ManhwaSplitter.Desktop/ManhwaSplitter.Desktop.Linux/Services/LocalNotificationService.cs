using System.Threading.Tasks;
using DesktopNotifications;
using DesktopNotifications.FreeDesktop;
using ErrorOr;
using ManhwaSplitter.Core.Services;

namespace ManhwaSplitter.Desktop.Linux.Services;

public class LocalNotificationService : ILocalNotificationService
{
    private readonly INotificationManager _notificationManager;

    public LocalNotificationService()
    {
        _notificationManager = new FreeDesktopNotificationManager();
        _notificationManager.Initialize();
    }

    public async Task<ErrorOr<Success>> Show(string title, string message)
    {
        await _notificationManager.ShowNotification(new Notification
        {
            Title = title,
            Body = message
        });

        return new Success();
    }
}