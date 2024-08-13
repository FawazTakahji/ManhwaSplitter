using Android.Views;
using ErrorOr;
using Microsoft.Maui.ApplicationModel;

namespace ManhwaSplitter.Mobile.Android.Services;

public class StatusBarServiceApi30 : IStatusBarService
{
    public ErrorOr<Success> SetStatusBarToLight()
    {
        if (Platform.CurrentActivity is not { } activity)
            return Error.Unexpected(description: "Couldn't retrieve the current activity.");
        if (activity.Window is not { } window)
            return Error.Unexpected(description: "Couldn't retrieve the window.");
        if (window.DecorView.WindowInsetsController is not { } windowInsetsController)
            return Error.Unexpected(description: "Couldn't retrieve the window insets controller.");

        windowInsetsController.SetSystemBarsAppearance((int)WindowInsetsControllerAppearance.None,
            (int)WindowInsetsControllerAppearance.LightStatusBars);
        return new Success();
    }
}