using ErrorOr;

namespace ManhwaSplitter.Core.Services;

public interface IUpdateService
{
    public Task<ErrorOr<Success>> CheckUpdate(bool showNotification = true);
}