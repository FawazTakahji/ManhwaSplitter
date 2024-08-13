using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ErrorOr;
using ManhwaSplitter.Core;
using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Core.Services;
using ManhwaSplitter.Core.ViewModels;
using ManhwaSplitter.Mobile.Views;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace ManhwaSplitter.Mobile;

public partial class App : Application
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    public static TopLevel? TopLevel { get; set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        MainView view = new()
        {
            DataContext = Singletons.ServiceProvider.GetRequiredService<MainViewModel>()
        };

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                Content = view
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = view;
        }

        base.OnFrameworkInitializationCompleted();

        CheckUpdate();
    }

    private static void CheckUpdate()
    {
        var settingsService = Singletons.ServiceProvider.GetService<ISettingsService>();
        if (settingsService is null)
        {
            Logger.Warn("Failed to check for updates on startup because the settings service is not available.");
            return;
        }
        if (!settingsService.CurrentSettings.CheckUpdateOnStartup)
            return;

        var updateService = Singletons.ServiceProvider.GetService<IUpdateService>();
        var appNotificationService = Singletons.ServiceProvider.GetService<IAppNotificationService>();
        if (updateService is null)
        {
            Logger.Warn("Failed to check for updates on startup because the update service is not available.");
            return;
        }

        Dispatcher.UIThread.InvokeAsync(async () =>
        {
            ErrorOr<Success> result = await updateService.CheckUpdate(false);
            if (!result.IsError)
                return;
            appNotificationService?.Show("Update Check Failed", result.FirstError.Description, NotificationType.Error);

            if (result.FirstError.Metadata?["Exception"] is not Exception ex)
                return;
            Logger.Error(ex, "An error occurred while checking for updates.");
        });
    }
}