using System;
using System.Threading.Tasks;
using ErrorOr;
using ManhwaSplitter.Core.Services;
using Microsoft.Maui.ApplicationModel;

namespace ManhwaSplitter.Mobile.Services;

public class LaunchService : ILaunchService
{
    public async Task<ErrorOr<Success>> OpenUrl(string url)
    {
        try
        {
            return await Launcher.OpenAsync(url) ? new Success() : Error.Failure(description: "Couldn't open the url.");
        }
        catch (Exception ex)
        {
            return Error.Unexpected(description: "An error occurred while opening the url.", metadata: new()
            {
                { "Exception", ex }
            });
        }
    }
}