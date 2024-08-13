using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using ManhwaSplitter.Desktop.Messages;
using ManhwaSplitter.Desktop.Services;

namespace ManhwaSplitter.Desktop.Views;

public partial class MainView : UserControl, IRecipient<NotificationMessage>
{
    private WindowNotificationManager? _notificationManager;

    public MainView()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.Register<NotificationMessage>(this);
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        TopLevel? topLevel = TopLevel.GetTopLevel(this);
        if (topLevel is null)
            return;

        _notificationManager = new WindowNotificationManager(topLevel)
        {
            Position = NotificationPosition.TopLeft
        };

        FileService.StorageProvider = topLevel.StorageProvider;
        LaunchService.Launcher = topLevel.Launcher;
    }

    public void Receive(NotificationMessage message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            _notificationManager?.Show(message.Value);
        });
    }
}