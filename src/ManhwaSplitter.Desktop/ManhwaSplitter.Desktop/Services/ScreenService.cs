using Avalonia.Controls;
using ErrorOr;
using ManhwaSplitter.Core.Services;

namespace ManhwaSplitter.Desktop.Services;

public class ScreenService : IScreenService
{
    public WindowBase? MainWindow;

    public ErrorOr<int> GetHeight()
    {
        if (MainWindow is null)
            return Error.Failure(description: "Couldn't retrieve the main window.");

        if (MainWindow.Screens.ScreenFromWindow(MainWindow) is not { } screen)
            return Error.Failure(description: "Couldn't get the screen from the current window instance.");

        return screen.Bounds.Height;
    }
}