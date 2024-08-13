using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.Messaging.Messages;

namespace ManhwaSplitter.Mobile.Messages;

public class NotificationMessage(Notification value) : ValueChangedMessage<Notification>(value);