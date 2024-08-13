using Avalonia.Controls;

namespace ManhwaSplitter.Desktop.Views;

public partial class AboutView : UserControl
{
    public AboutView()
    {
        InitializeComponent();

        TbVersion.Text = typeof(AboutView).Assembly.GetName().Version?.ToString(3) ?? "Unknown Version";
    }
}