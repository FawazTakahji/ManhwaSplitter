using System;
using Avalonia;
using ManhwaSplitter.Core;
using ManhwaSplitter.Core.Extensions;
using ManhwaSplitter.Core.Services;
using ManhwaSplitter.Mobile.Extensions;
using ManhwaSplitter.Mobile.Preview.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ManhwaSplitter.Mobile.Preview;

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
        ConfigureServices();

        return AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }

    private static void ConfigureServices()
    {
        ServiceCollection collection = [];
        collection.AddCommonServices();
        collection.AddMobileServices();

        collection.AddSingleton<IFileService, FileService>();
        collection.AddSingleton<IScreenService, ScreenService>();

        Singletons.ServiceProvider = collection.BuildServiceProvider();
    }
}