using Android.Util;
using ErrorOr;
using ManhwaSplitter.Core.Services;
using Microsoft.Maui.ApplicationModel;

namespace ManhwaSplitter.Mobile.Android.Services;

public class ScreenServiceApi21 : IScreenService
{
    public ErrorOr<int> GetHeight()
    {
        if (Platform.CurrentActivity is not {} activity)
            return Error.Unexpected(description: "Couldn't retrieve the current activity.");

        if (activity.WindowManager is not { } windowManager)
            return Error.Unexpected(description: "Couldn't retrieve the window manager.");

        if (windowManager.DefaultDisplay is not {} display)
            return Error.Unexpected(description: "Couldn't retrieve the default display.");

        DisplayMetrics metrics = new();
        display.GetRealMetrics(metrics);

        return metrics.HeightPixels;
    }
}