using System;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ErrorOr;
using ManhwaSplitter.Core.Services;

namespace ManhwaSplitter.Desktop.Services;

public class LaunchService : ILaunchService
{
    public static ILauncher? Launcher { get; set; }

    public async Task<ErrorOr<Success>> OpenUrl(string url)
    {
        if (Launcher is null)
            return Error.Failure(description: "Couldn't get the launcher instance.");
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri))
            return Error.Failure(description: "The url is not valid.");

        return await Launcher.LaunchUriAsync(uri) ? new Success() : Error.Failure(description: "Couldn't open the url.");
    }
}