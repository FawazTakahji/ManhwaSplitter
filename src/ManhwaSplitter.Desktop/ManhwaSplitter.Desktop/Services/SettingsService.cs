using System;
using System.ComponentModel;
using System.IO;
using System.Text.Json;
using Avalonia;
using Avalonia.Styling;
using ErrorOr;
using ManhwaSplitter.Core;
using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Core.Models;
using ManhwaSplitter.Core.Services;
using NLog;

namespace ManhwaSplitter.Desktop.Services;

public class SettingsService : ISettingsService
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly IAppNotificationService _appNotificationService;
    private static readonly string DataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");
    private static readonly string SettingsPath = Path.Combine(DataPath, "settings.json");

    public Settings CurrentSettings { get; }

    public SettingsService(IAppNotificationService appNotificationService)
    {
        _appNotificationService = appNotificationService;

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

        if (!File.Exists(SettingsPath))
        {
            Save(settings);
            return settings;
        }

        try
        {
            string json = File.ReadAllText(SettingsPath);
            settings = JsonSerializer.Deserialize<Settings>(json) ?? settings;
            return settings;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "An error occurred while loading settings.");
            return Error.Failure(description: "An error occurred while loading settings.");
        }
    }

    public ErrorOr<Success> Save(Settings settings)
    {
        try
        {
            string json = JsonSerializer.Serialize(settings, Singletons.JsonSerializerOptions);
            Directory.CreateDirectory(DataPath);
            File.WriteAllText(SettingsPath, json);
            return new Success();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "An error occurred while saving settings.");
            return Error.Failure(description: "An error occurred while saving settings.");
        }
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