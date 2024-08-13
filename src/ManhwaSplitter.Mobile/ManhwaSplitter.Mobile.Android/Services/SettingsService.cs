using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Styling;
using ErrorOr;
using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Core.Models;
using ManhwaSplitter.Core.Services;
using Microsoft.Maui.Storage;
using NLog;

namespace ManhwaSplitter.Mobile.Android.Services;

public class SettingsService : ISettingsService
{
    private readonly IAppNotificationService _appNotificationService;
    private readonly IStatusBarService _statusBarService;
    public Settings CurrentSettings { get; }

    public SettingsService(IAppNotificationService appNotificationService, IStatusBarService statusBarService)
    {
        _appNotificationService = appNotificationService;
        _statusBarService = statusBarService;

        ErrorOr<Settings> settings = Load();
        if (settings.IsError)
        {
            CurrentSettings = new();
            _appNotificationService.Show("Error", settings.FirstError.Description, NotificationType.Error);
            return;
        }

        CurrentSettings = settings.Value;
        ErrorOr<Success> result = Apply(CurrentSettings);
        if (result.IsError)
        {
            _appNotificationService.Show("Error", result.FirstError.Description, NotificationType.Error);
        }

        CurrentSettings.PropertyChanged += OnSettingsChanged;
    }

    private void OnSettingsChanged(object? sender, PropertyChangedEventArgs e)
    {
        ErrorOr<Success> applyResult = Apply(CurrentSettings);
        if (applyResult.IsError)
        {
            _appNotificationService.Show("Error", applyResult.FirstError.Description, NotificationType.Error);
        }

        ErrorOr<Success> saveResult = Save(CurrentSettings);
        if (saveResult.IsError)
        {
            _appNotificationService.Show("Error", saveResult.FirstError.Description, NotificationType.Error);
        }
    }

    public ErrorOr<Settings> Load()
    {
        Settings settings = new();

        string themeString = Preferences.Get("Theme", Theme.System.ToString());
        if (Enum.TryParse(themeString, out Theme theme))
            settings.Theme = theme;

        settings.MaxHeight = Preferences.Get("MaxHeight", 2340);
        settings.SimultaneousOperations = Preferences.Get("SimultaneousOperations", 4);
        settings.CheckUpdateOnStartup = Preferences.Get("CheckUpdateOnStartup", true);

        return settings;
    }

    public ErrorOr<Success> Save(Settings settings)
    {
        Preferences.Set("Theme", settings.Theme.ToString());
        Preferences.Set("MaxHeight", settings.MaxHeight);
        Preferences.Set("SimultaneousOperations", settings.SimultaneousOperations);
        Preferences.Set("CheckUpdateOnStartup", settings.CheckUpdateOnStartup);
        return new Success();
    }

    public ErrorOr<Success> Apply(Settings settings)
    {
        if (Application.Current is null)
        {
            return Error.Failure(description: "Failed to get the current application instance.");
        }

        switch (settings.Theme)
        {
            case Theme.System:
                Application.Current.RequestedThemeVariant = ThemeVariant.Default;
                break;
            case Theme.Light:
                Application.Current.RequestedThemeVariant = ThemeVariant.Light;
                break;
            case Theme.Dark:
                Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
                break;
        }

        ErrorOr<Success> statusBar = _statusBarService.SetStatusBarToLight();
        if (statusBar.IsError)
            LogManager.GetCurrentClassLogger().Warn($"Couldn't set the status bar to light theme: {statusBar.FirstError.Description}");

        return new Success();
    }

    public void Reset()
    {
        CurrentSettings.PropertyChanged -= OnSettingsChanged;

        CurrentSettings.Theme = Theme.System;
        CurrentSettings.MaxHeight = 2340;
        CurrentSettings.SimultaneousOperations = 4;
        CurrentSettings.CheckUpdateOnStartup =
#if DEBUG
            false;
#else
            true;
#endif

        CurrentSettings.PropertyChanged += OnSettingsChanged;

        Save(CurrentSettings);
        Apply(CurrentSettings);
    }
}