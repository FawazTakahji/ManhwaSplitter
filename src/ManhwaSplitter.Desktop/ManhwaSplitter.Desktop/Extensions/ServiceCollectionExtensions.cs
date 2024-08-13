using ManhwaSplitter.Core.Services;
using ManhwaSplitter.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ManhwaSplitter.Desktop.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddDesktopServices(this IServiceCollection collection)
    {
        collection.AddSingleton<IFileService, FileService>();
        collection.AddSingleton<IAppNotificationService, AppNotificationService>();
        collection.AddSingleton<IScreenService, ScreenService>();
        collection.AddSingleton<ISettingsService, SettingsService>();
        collection.AddSingleton<IDialogService, DialogService>();
        collection.AddSingleton<ILaunchService, LaunchService>();
        collection.AddSingleton<IPermissionService, PermissionService>();
    }
}