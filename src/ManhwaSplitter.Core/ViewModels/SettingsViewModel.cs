using CommunityToolkit.Mvvm.Input;
using ErrorOr;
using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Core.Models;
using ManhwaSplitter.Core.Services;

namespace ManhwaSplitter.Core.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    public ISettingsService SettingsService { get; }
    private readonly IAppNotificationService _appNotificationService;
    private readonly IScreenService _screenService;

    public SettingsViewModel(ISettingsService settingsService, IAppNotificationService appNotificationService, IScreenService screenService)
    {
        SettingsService = settingsService;
        _appNotificationService = appNotificationService;
        _screenService = screenService;
    }

    [RelayCommand]
    private void GetScreenHeight()
    {
        ErrorOr<int> height = _screenService.GetHeight();
        if (height.IsError)
        {
            _appNotificationService.Show("Error", height.FirstError.Description, NotificationType.Error);
            return;
        }

        SettingsService.CurrentSettings.MaxHeight = height.Value;
    }

    [RelayCommand]
    private void GetAvailableProcessors()
    {
        SettingsService.CurrentSettings.SimultaneousOperations = Environment.ProcessorCount;
    }

    [RelayCommand]
    private void ResetSettings()
    {
        SettingsService.Reset();
    }

#if DEBUG
    public SettingsViewModel()
    {
        SettingsService = new PreviewSettingsService();
    }

    private class PreviewSettingsService : ISettingsService
    {
        public Settings CurrentSettings { get; } = new();

        public ErrorOr<Settings> Load() => throw new NotImplementedException();
        public ErrorOr<Success> Save(Settings settings) => throw new NotImplementedException();
        public ErrorOr<Success> Apply(Settings settings) => throw new NotImplementedException();
        public void Reset() => throw new NotImplementedException();
    }
#endif
}