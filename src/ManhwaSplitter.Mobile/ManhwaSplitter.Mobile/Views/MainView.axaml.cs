using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Interactivity;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Messaging;
using ManhwaSplitter.Mobile.Messages;

namespace ManhwaSplitter.Mobile.Views;

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

        App.TopLevel = topLevel;
        _notificationManager = new WindowNotificationManager(topLevel)
        {
            Position = NotificationPosition.TopCenter,
            Margin = new Thickness(0)
        };
    }

    public void Receive(NotificationMessage message)
    {
        Dispatcher.UIThread.Invoke(() =>
        {
            _notificationManager?.Show(message.Value);
        });
    }
}