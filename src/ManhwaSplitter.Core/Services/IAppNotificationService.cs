using ManhwaSplitter.Core.Enums;

namespace ManhwaSplitter.Core.Services;

public interface IAppNotificationService
{
    public void Show(string title, string message, NotificationType type = NotificationType.Information, TimeSpan? expiration = null);
}