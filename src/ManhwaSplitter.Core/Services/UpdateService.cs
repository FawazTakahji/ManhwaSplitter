using ErrorOr;
using ManhwaSplitter.Core.Enums;
using Octokit;

namespace ManhwaSplitter.Core.Services;

public class UpdateService : IUpdateService
{
    private readonly IGitHubClient _client;
    private readonly IDialogService _dialogService;
    private readonly ILaunchService _launchService;
    private readonly IAppNotificationService _appNotificationService;

    public UpdateService(IGitHubClient client,
        IDialogService dialogService,
        ILaunchService launchService,
        IAppNotificationService appNotificationService)
    {
        _client = client;
        _dialogService = dialogService;
        _launchService = launchService;
        _appNotificationService = appNotificationService;
    }

    public async Task<ErrorOr<Success>> CheckUpdate(bool showNotification = true)
    {
        Version? currentVersion = typeof(UpdateService).Assembly.GetName().Version;
        if (currentVersion is null)
            return Error.Failure(description: "Couldn't retrieve the current version.");

        ErrorOr<(Version version, string releaseUrl)> release = await GetLatestVersion();
        if (release.IsError)
            return release.FirstError;
        if (release.Value.version <= currentVersion)
        {
            if (showNotification)
                _appNotificationService.Show("No Update Available", "You are using the latest version.", NotificationType.Success);

            return new Success();
        }

        bool result = await _dialogService.ShowYesNoDialog("Update Avaiable",
            $"There is a new version available ({release.Value.version}).\nDo you want to update?",
            DialogType.Question);
        if (!result)
            return new Success();

        return await _launchService.OpenUrl(release.Value.releaseUrl);
    }

    private async Task<ErrorOr<(Version, string)>> GetLatestVersion()
    {
        ErrorOr<Release> latestRelease = await GetLatestRelease();

        if (latestRelease.IsError)
            return latestRelease.FirstError;

        try
        {
            return (new Version(latestRelease.Value.TagName), latestRelease.Value.HtmlUrl);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: "An error occurred while parsing the latest release tag.", metadata: new()
            {
                { "Exception", ex }
            });
        }
    }

    private async Task<ErrorOr<Release>> GetLatestRelease()
    {
        try
        {
            return await _client.Repository.Release.GetLatest("FawazTakahji", "ManhwaSplitter");
        }
        catch (NotFoundException)
        {
            return Error.Failure(description: "The release or repository could not be found.");
        }
        catch (Exception ex)
        {
            return Error.Failure(description: "An error occurred while retrieving the latest release.", metadata: new()
            {
                { "Exception", ex }
            });
        }
    }
}