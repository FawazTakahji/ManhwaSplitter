using System.Collections.Generic;
using System.Collections.ObjectModel;
using ManhwaSplitter.Core.ViewModels;

namespace ManhwaSplitter.Mobile.Dialogs.ViewModels;

public class FoldersPickerViewModel : ViewModelBase
{
    public List<string> Folders { get; }
    public ObservableCollection<string> SelectedFolders { get; }

    public FoldersPickerViewModel(List<string> folders)
    {
        Folders = folders;
        SelectedFolders = [];
    }

#if DEBUG
    public FoldersPickerViewModel()
    {
        Folders =
        [
            "C:/Users/User/Documents",
            "C:/Users/User/Pictures",
            "C:/Users/User/Downloads",
            "C:/Users/User/Desktop",
            "C:/Users/User/Music",
            "C:/Users/User/Videos",
        ];
        SelectedFolders = [];
    }
#endif
}