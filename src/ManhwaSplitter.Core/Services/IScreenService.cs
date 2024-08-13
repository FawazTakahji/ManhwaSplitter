using ErrorOr;

namespace ManhwaSplitter.Core.Services;

public interface IScreenService
{
    public ErrorOr<int> GetHeight();
}