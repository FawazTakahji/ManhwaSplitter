using Avalonia.Controls;
using WindowHelper = ManhwaSplitter.Desktop.Helpers.Window;

namespace ManhwaSplitter.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        WindowHelper.RestoreLocation(this);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);

        WindowHelper.SaveLocation(this);
    }
}