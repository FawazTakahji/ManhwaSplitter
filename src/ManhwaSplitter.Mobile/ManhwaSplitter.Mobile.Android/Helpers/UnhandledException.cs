using Android.Widget;
using System;
using Android.OS;
using Microsoft.Maui.ApplicationModel;
using NLog;

namespace ManhwaSplitter.Mobile.Android.Helpers;

public static class UnhandledException
{
    public static void Handle(this Exception exception)
    {
        LogManager.GetCurrentClassLogger()
            .Fatal(exception, "Unhandled exception.");

        if (Build.VERSION.SdkInt >= BuildVersionCodes.Tiramisu)
            ShowToast();
        else
            ShowNotification();
    }

    private static void ShowToast()
    {
        Toast? toast = Toast.MakeText(Platform.AppContext,
            "App crashed. Please check the logs for more information.", ToastLength.Long);
        toast?.Show();
    }

    private static void ShowNotification()
    {
        Notification.ShowNotification("Manhwa Splitter", "App crashed. Please check the logs for more information.",
            Notification.Crashes, 0);
    }
}