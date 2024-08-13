using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Notifications;
using Avalonia.Input;
using CommunityToolkit.Mvvm.Messaging;
using ManhwaSplitter.Core.ViewModels;
using ManhwaSplitter.Desktop.Messages;

namespace ManhwaSplitter.Desktop.Views;

public partial class HomeView : UserControl
{
    public HomeView()
    {
        InitializeComponent();

        SplitImagesArea.AddHandler(DragDrop.DragOverEvent, Border_DragOver);
        SplitImagesArea.AddHandler(DragDrop.DropEvent, ImagesBorder_Drop);

        SplitFoldersArea.AddHandler(DragDrop.DragOverEvent, Border_DragOver);
        SplitFoldersArea.AddHandler(DragDrop.DropEvent, FoldersBorder_Drop);
    }

    private void Border_DragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects &= DragDropEffects.Copy;
        if (!e.Data.Contains(DataFormats.Files)
            || DataContext is not HomeViewModel viewModel
            || !viewModel.CanSplit())
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void ImagesBorder_Drop(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains(DataFormats.Files))
        {
            WeakReferenceMessenger.Default.Send(
                new NotificationMessage(
                    new Notification("Error", "The dropped data does not contain any files.", NotificationType.Error)));
            return;
        }

        if (e.Data.GetFiles() is not { } storageItems
            || storageItems.Select(file => file.Path.LocalPath).ToList() is not { } files)
        {
            WeakReferenceMessenger.Default.Send(
                new NotificationMessage(
                    new Notification("Error", "Failed to retrieve the dropped files.", NotificationType.Error)));
            return;
        }

        if (DataContext is not HomeViewModel viewModel)
        {
            WeakReferenceMessenger.Default.Send(
                new NotificationMessage(
                    new Notification("Error", "Failed to retrieve the view model.", NotificationType.Error)));
            return;
        }

        if (!viewModel.SplitImagesCommand.CanExecute(null))
        {
            WeakReferenceMessenger.Default.Send(
                new NotificationMessage(
                    new Notification("Error", "The Split Images command cannot be executed.", NotificationType.Error)));
            return;
        }

        viewModel.SplitImagesCommand.Execute(files);
    }

    private void FoldersBorder_Drop(object? sender, DragEventArgs e)
    {
        if (!e.Data.Contains(DataFormats.Files))
        {
            WeakReferenceMessenger.Default.Send(
                new NotificationMessage(
                    new Notification("Error", "The dropped data does not contain any folders.", NotificationType.Error)));
            return;
        }

        if (e.Data.GetFiles() is not { } storageItems
            || storageItems.Select(file => file.Path.LocalPath).ToList() is not { } folders)
        {
            WeakReferenceMessenger.Default.Send(
                new NotificationMessage(
                    new Notification("Error", "Failed to retrieve the dropped folders.", NotificationType.Error)));
            return;
        }

        if (DataContext is not HomeViewModel viewModel)
        {
            WeakReferenceMessenger.Default.Send(
                new NotificationMessage(
                    new Notification("Error", "Failed to retrieve the view model.", NotificationType.Error)));
            return;
        }

        if (!viewModel.SplitFoldersCommand.CanExecute(null))
        {
            WeakReferenceMessenger.Default.Send(
                new NotificationMessage(
                    new Notification("Error", "The Split Folders command cannot be executed.", NotificationType.Error)));
            return;
        }

        viewModel.SplitFoldersCommand.Execute(folders);
    }
}