using ManhwaSplitter.Core.Models;
using ManhwaSplitter.Core.ViewModels;

namespace ManhwaSplitter.Core.Dialogs.ViewModels;

public class FailedOperationsViewModel : ViewModelBase
{
    public FailedFolder[] FailedOperations { get; }

    public FailedOperationsViewModel(FailedFolder[] failedOperations)
    {
        FailedOperations = failedOperations;
    }

#if DEBUG
    public FailedOperationsViewModel()
    {
        FailedOperations =
        [
            new FailedFolder("Folder 1", files:
            [
                new FailedFile("File 1.png", "Lorem ipsum dolor sit amet, consectetur adipiscing elit."),
                new FailedFile("File 2.png", "Lorem ipsum dolor sit amet, consectetur adipiscing elit."),
                new FailedFile("File 3.png", "Lorem ipsum dolor sit amet, consectetur adipiscing elit.")
            ]),
            new FailedFolder("Folder 2", "Lorem ipsum dolor sit amet, consectetur adipiscing elit."),
            new FailedFolder("Folder 3", "Lorem ipsum dolor sit amet, consectetur adipiscing elit.")
        ];
    }
#endif
}