using ManhwaSplitter.Core.Services;
using ManhwaSplitter.Core.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Octokit;

namespace ManhwaSplitter.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IUpdateService, UpdateService>();

        services.AddSingleton<MainViewModel>();
        services.AddSingleton<HomeViewModel>();
        services.AddSingleton<SettingsViewModel>();
        services.AddSingleton<AboutViewModel>();

        GitHubClient client = new(new ProductHeaderValue("ManhwaSplitter"));
        services.AddSingleton(typeof(IGitHubClient), client);
    }
}