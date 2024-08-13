using ErrorOr;
using ManhwaSplitter.Core.Services;

namespace ManhwaSplitter.Mobile.Preview.Services;

public class ScreenService : IScreenService
{
    public ErrorOr<int> GetHeight()
    {
        return Error.Failure();
    }
}