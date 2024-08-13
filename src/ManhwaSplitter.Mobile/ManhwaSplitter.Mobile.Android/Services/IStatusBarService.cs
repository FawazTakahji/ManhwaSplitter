using ErrorOr;

namespace ManhwaSplitter.Mobile.Android.Services;

public interface IStatusBarService
{
    public ErrorOr<Success> SetStatusBarToLight();
}