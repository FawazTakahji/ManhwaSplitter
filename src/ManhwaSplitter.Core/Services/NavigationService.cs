using CommunityToolkit.Mvvm.ComponentModel;
using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Core.ViewModels;

namespace ManhwaSplitter.Core.Services;

public partial class NavigationService : ObservableObject, INavigationService
{
    private readonly HomeViewModel _homeViewModel;
    private readonly SettingsViewModel _settingsViewModel;
    private readonly AboutViewModel _aboutViewModel;

    [ObservableProperty]
    private ViewModelBase? _currentView;

    public NavigationService(HomeViewModel homeViewModel, SettingsViewModel settingsViewModel, AboutViewModel aboutViewModel)
    {
        _homeViewModel = homeViewModel;
        _settingsViewModel = settingsViewModel;
        _aboutViewModel = aboutViewModel;
    }

    public void Navigate(View view)
    {
        CurrentView = view switch
        {
            View.Home => _homeViewModel,
            View.Settings => _settingsViewModel,
            View.About => _aboutViewModel,

            _ => _homeViewModel
        };
    }
}