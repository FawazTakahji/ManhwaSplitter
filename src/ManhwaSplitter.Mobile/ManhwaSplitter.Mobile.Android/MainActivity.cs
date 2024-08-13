using System;
using System.IO;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using ErrorOr;
using ManhwaSplitter.Core;
using ManhwaSplitter.Core.Extensions;
using ManhwaSplitter.Core.Services;
using Microsoft.Maui.ApplicationModel;
using ManhwaSplitter.Mobile.Android.Helpers;
using ManhwaSplitter.Mobile.Android.Services;
using ManhwaSplitter.Mobile.Extensions;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Config;
using NLog.Targets;
using Exception = System.Exception;
using Notification = ManhwaSplitter.Mobile.Android.Helpers.Notification;
using Result = Android.App.Result;

namespace ManhwaSplitter.Mobile.Android;

[Activity(
    Label = "Manhwa Splitter",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    private IStatusBarService? _statusBarService;

    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, e)
            => ((Exception)e.ExceptionObject).Handle();

        ConfigureNLog();
        ConfigureServices();
        _statusBarService = Singletons.ServiceProvider.GetRequiredService<IStatusBarService>();

        return base.CustomizeAppBuilder(builder)
            .WithInterFont();
    }

    protected override void OnCreate(Bundle? savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Platform.Init(this, savedInstanceState);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O && Build.VERSION.SdkInt < BuildVersionCodes.Tiramisu)
            Notification.CreateNotificationChannels();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        Process.KillProcess(Process.MyPid());
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
    {
        Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
    }

    protected override void OnPostResume()
    {
        base.OnPostResume();

        ErrorOr<Success>? result = _statusBarService?.SetStatusBarToLight();
        if (result is { IsError: true })
            LogManager.GetCurrentClassLogger().Warn($"Couldn't set the status bar to light theme: {result.Value.FirstError.Description}");
    }

    protected override void OnActivityResult(int requestCode, Result resultCode, Intent? data)
    {
        if (requestCode == Storage.RequestCode)
            Storage.OnActivityResult();

        base.OnActivityResult(requestCode, resultCode, data);
    }

    private static void ConfigureNLog()
    {
        string? filesDir = Platform.AppContext.GetExternalFilesDir(null)?.AbsolutePath;
        if (filesDir is null)
            return;

        LoggingConfiguration config = new();
        FileTarget fileTarget = new()
        {
            FileName = Path.Combine(filesDir, "logs.txt"),
            DeleteOldFileOnStartup = true
        };
        config.AddTarget("file", fileTarget);
        config.AddRule(new LoggingRule("*", LogLevel.Trace, fileTarget));
        LogManager.Configuration = config;
        LogManager.GetCurrentClassLogger().Info("NLog configured.");
    }

    private static void ConfigureServices()
    {
        ServiceCollection collection = [];
        collection.AddCommonServices();
        collection.AddMobileServices();

        collection.AddSingleton<IFileService, FileService>();
        collection.AddSingleton<ISettingsService, SettingsService>();

        if (Build.VERSION.SdkInt < BuildVersionCodes.R)
        {
            collection.AddSingleton<IPermissionService, PermissionServiceApi23>();
            collection.AddSingleton<IScreenService, ScreenServiceApi21>();
            collection.AddSingleton<IStatusBarService, StatusBarServiceApi23>();
        }
        else
        {
            collection.AddSingleton<IPermissionService, PermissionServiceApi30>();
            collection.AddSingleton<IScreenService, ScreenServiceApi30>();
            collection.AddSingleton<IStatusBarService, StatusBarServiceApi30>();
        }

        Singletons.ServiceProvider = collection.BuildServiceProvider();
    }
}