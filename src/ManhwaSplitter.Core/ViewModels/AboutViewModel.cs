using CommunityToolkit.Mvvm.Input;
using ManhwaSplitter.Core.Services;
using ErrorOr;
using ManhwaSplitter.Core.Enums;
using NLog;

namespace ManhwaSplitter.Core.ViewModels;

public partial class AboutViewModel : ViewModelBase
{
    private readonly Logger _logger = LogManager.GetCurrentClassLogger();
    private readonly IUpdateService _updateService;
    private readonly IAppNotificationService _notificationService;
    public ISettingsService SettingsService { get; }

    public AboutViewModel(IUpdateService updateService, IAppNotificationService notificationService, ISettingsService settingsService)
    {
        _updateService = updateService;
        _notificationService = notificationService;
        SettingsService = settingsService;
    }

    [RelayCommand]
    private async Task CheckUpdates()
    {
        ErrorOr<Success> result = await _updateService.CheckUpdate();
        if (!result.IsError)
            return;

        _notificationService.Show("Check Failed", result.FirstError.Description, NotificationType.Error);

        if (result.FirstError.Metadata?["Exception"] is Exception ex)
            _logger.Error(ex, "An error occurred while checking for updates.");
    }

#if DEBUG
    public AboutViewModel()
    {
    }
#endif
}