using ErrorOr;
using ManhwaSplitter.Core.Services;
using Microsoft.Maui.ApplicationModel;

namespace ManhwaSplitter.Mobile.Android.Services;

public class ScreenServiceApi30 : IScreenService
{
    public ErrorOr<int> GetHeight()
    {
        if (Platform.CurrentActivity is not {} activity)
            return Error.Unexpected(description: "Couldn't retrieve the current activity.");

        if (activity.WindowManager is not { } windowManager)
            return Error.Unexpected(description: "Couldn't retrieve the window manager.");

        return windowManager.CurrentWindowMetrics.Bounds.Height();
    }
}