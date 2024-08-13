using ErrorOr;

namespace ManhwaSplitter.Core.Services;

public interface ILaunchService
{
    public Task<ErrorOr<Success>> OpenUrl(string url);
}