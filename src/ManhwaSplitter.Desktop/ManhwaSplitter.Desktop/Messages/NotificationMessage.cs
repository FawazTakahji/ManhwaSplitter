using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ManhwaSplitter.Desktop.Messages;

public class NotificationMessage(Notification value) : ValueChangedMessage<Notification>(value);