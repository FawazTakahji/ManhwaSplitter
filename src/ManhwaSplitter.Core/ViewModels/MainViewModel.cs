using CommunityToolkit.Mvvm.ComponentModel;
using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Core.Models;
using ManhwaSplitter.Core.Services;

namespace ManhwaSplitter.Core.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public INavigationService NavigationService { get; }
    public NavigationItem[] NavigationItems { get; }
    [ObservableProperty]
    private NavigationItem _selectedNavigationItem;

    public MainViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
        NavigationItems =
        [
            new NavigationItem("Home", View.Home),
            new NavigationItem("Settings", View.Settings),
            new NavigationItem("About", View.About)
        ];
        SelectedNavigationItem = NavigationItems[0];
    }

    partial void OnSelectedNavigationItemChanged(NavigationItem value)
    {
        NavigationService.Navigate(value.View);
    }

#if DEBUG
    public MainViewModel()
    {
        NavigationService = new NavigationService(new HomeViewModel(), new SettingsViewModel(), new AboutViewModel());
        NavigationItems =
        [
            new NavigationItem("Home", View.Home),
            new NavigationItem("Settings", View.Settings),
            new NavigationItem("About", View.About)
        ];
        SelectedNavigationItem = NavigationItems[0];
    }
#endif
}