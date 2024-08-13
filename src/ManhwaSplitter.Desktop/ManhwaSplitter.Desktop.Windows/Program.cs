using System;
using Avalonia;
using ManhwaSplitter.Core;
using ManhwaSplitter.Core.Extensions;
using ManhwaSplitter.Core.Services;
using ManhwaSplitter.Desktop.Extensions;
using ManhwaSplitter.Desktop.Helpers;
using ManhwaSplitter.Desktop.Windows.Services;
using Microsoft.Extensions.DependencyInjection;
using Projektanker.Icons.Avalonia;
using Projektanker.Icons.Avalonia.FontAwesome;

namespace ManhwaSplitter.Desktop.Windows;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
    {
        Logging.ConfigureNLog();
        ConfigureServices();

        AppDomain.CurrentDomain.UnhandledException += async (_, e)
            => await ((Exception)e.ExceptionObject).Handle();

        IconProvider.Current.Register<FontAwesomeIconProvider>();

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }

    private static void ConfigureServices()
    {
        ServiceCollection collection = [];
        collection.AddCommonServices();
        collection.AddDesktopServices();

        collection.AddSingleton<ILocalNotificationService, LocalNotificationService>();

        Singletons.ServiceProvider = collection.BuildServiceProvider();
    }
}