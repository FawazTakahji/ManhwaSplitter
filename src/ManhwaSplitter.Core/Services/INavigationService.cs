using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Core.ViewModels;

namespace ManhwaSplitter.Core.Services;

public interface INavigationService
{
    public ViewModelBase CurrentView { get; set; }

    public void Navigate(View view);
}