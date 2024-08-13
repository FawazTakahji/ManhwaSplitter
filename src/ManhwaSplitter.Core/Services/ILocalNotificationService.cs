using ErrorOr;

namespace ManhwaSplitter.Core.Services;

public interface ILocalNotificationService
{
    public Task<ErrorOr<Success>> Show(string title, string message);
}