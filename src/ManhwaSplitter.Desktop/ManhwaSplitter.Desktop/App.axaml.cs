using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using ErrorOr;
using ManhwaSplitter.Core;
using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Core.Services;
using ManhwaSplitter.Core.ViewModels;
using ManhwaSplitter.Desktop.Services;
using ManhwaSplitter.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace ManhwaSplitter.Desktop;

public partial class App : Application
{
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
            desktop.MainWindow = new MainWindow(desktop.Args)
            {
                Content = view
            };

            ((ScreenService)Singletons.ServiceProvider.GetRequiredService<IScreenService>()).MainWindow = desktop.MainWindow;
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
        Logger logger = LogManager.GetCurrentClassLogger();
        var settingsService = Singletons.ServiceProvider.GetService<ISettingsService>();
        if (settingsService is null)
        {
            logger.Warn("Failed to check for updates on startup because the settings service is not available.");
            return;
        }
        if (!settingsService.CurrentSettings.CheckUpdateOnStartup)
            return;

        var updateService = Singletons.ServiceProvider.GetService<IUpdateService>();
        var appNotificationService = Singletons.ServiceProvider.GetService<IAppNotificationService>();
        if (updateService is null)
        {
            logger.Warn("Failed to check for updates on startup because the update service is not available.");
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
            logger.Error(ex, "An error occurred while checking for updates.");
        });
    }
}