using ManhwaSplitter.Core.Services;
using ManhwaSplitter.Mobile.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ManhwaSplitter.Mobile.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddMobileServices(this IServiceCollection services)
    {
        services.AddSingleton<IAppNotificationService, AppNotificationService>();
        services.AddSingleton<IDialogService, DialogService>();
        services.AddSingleton<ILaunchService, LaunchService>();
    }
}