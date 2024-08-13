using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using ManhwaSplitter.Core;
using ManhwaSplitter.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace ManhwaSplitter.Desktop.Helpers;

public static class UnhandledException
{
    public static async Task Handle(this Exception exception)
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        logger.Fatal(exception, "Unhandled exception.");
        Console.WriteLine($"Unhandled exception: {exception}");

        var notificationService = Singletons.ServiceProvider.GetService<ILocalNotificationService>();
        if (notificationService is null)
        {
            logger.Warn("Couldn't retrieve the notification service. The notification won't be shown.");
            return;
        }

        await notificationService.Show("App crashed", "Please check the logs for more information.");

        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            lifetime.Shutdown();
    }
}