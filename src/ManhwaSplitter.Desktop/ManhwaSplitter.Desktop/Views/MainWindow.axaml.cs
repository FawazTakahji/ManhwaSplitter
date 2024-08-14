using System;
using System.Linq;
using Avalonia.Controls;
using WindowHelper = ManhwaSplitter.Desktop.Helpers.Window;

namespace ManhwaSplitter.Desktop.Views;

public partial class MainWindow : Window
{
    public MainWindow(string[]? args)
    {
        InitializeComponent();

        if (args is null || args.All(arg =>!arg.Equals("--norestore", StringComparison.OrdinalIgnoreCase)))
            WindowHelper.RestoreLocation(this);
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);

        WindowHelper.SaveLocation(this);
    }

#if DEBUG
    public MainWindow()
    {
        InitializeComponent();
    }
#endif
}