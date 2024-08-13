using Android.Views;
using ErrorOr;
using Microsoft.Maui.ApplicationModel;

namespace ManhwaSplitter.Mobile.Android.Services;

public class StatusBarServiceApi23 : IStatusBarService
{
    public ErrorOr<Success> SetStatusBarToLight()
    {
        if (Platform.CurrentActivity is not { } activity)
            return Error.Unexpected(description: "Couldn't retrieve the current activity.");
        if (activity.Window is not { } window)
            return Error.Unexpected(description: "Couldn't retrieve the window.");

        window.DecorView.SystemUiFlags &= ~SystemUiFlags.LightStatusBar;
        return new Success();
    }
}