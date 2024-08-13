using Avalonia.Interactivity;
using AvaloniaDialogs.Views;

namespace ManhwaSplitter.Mobile.Dialogs.Views;

public partial class FailedOperationsView : BaseDialog
{
    public FailedOperationsView()
    {
        InitializeComponent();
    }

    private void OkButton_OnClick(object? sender, RoutedEventArgs e) => this.Close(null);
}