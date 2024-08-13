using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Microsoft.Maui.ApplicationModel;
using NLog;

namespace ManhwaSplitter.Mobile.Android.Helpers;

public static class Notification
{
    public const string Crashes = "1";

    public static void CreateNotificationChannels()
    {
        NotificationManager? manager =  (NotificationManager?)Application.Context.GetSystemService(Context.NotificationService);

        if (manager is null)
        {
            LogManager.GetCurrentClassLogger().Warn("Couldn't create the notification channel because the notification manager is null.");
            return;
        }

        CreateCrashesChannel(manager);
    }

    private static void CreateCrashesChannel(NotificationManager manager)
    {
        NotificationChannel channel = new NotificationChannel(Crashes, "App Crashes", NotificationImportance.Max)
        {
            Description = "Notifications to alert the user of app crashes."
        };

        manager.CreateNotificationChannel(channel);
    }

    public static void ShowNotification(string title, string message, string channelId, int notificationId)
    {
        var builder = new NotificationCompat.Builder(Platform.AppContext, channelId)
            .SetPriority(NotificationCompat.PriorityMax)
            .SetDefaults((int)NotificationDefaults.All)
            .SetStyle(new NotificationCompat.BigTextStyle().BigText(message))
            .SetSmallIcon(Resource.Drawable.Icon)
            .SetContentTitle(title)
            .SetContentText(message);

        NotificationManagerCompat.From(Platform.AppContext)
            .Notify(notificationId, builder.Build());
    }
}