using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Interactivity;
using AvaloniaDialogs.Views;
using ManhwaSplitter.Mobile.Dialogs.ViewModels;
using ErrorOr;

namespace ManhwaSplitter.Mobile.Dialogs.Views;

public partial class FoldersPickerView : BaseDialog<ErrorOr<List<string>>>
{
    public FoldersPickerView()
    {
        InitializeComponent();
    }

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is not FoldersPickerViewModel viewModel)
            return;

        viewModel.SelectedFolders.CollectionChanged += SelectedFoldersChanged;
    }

    private void SelectAllButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not FoldersPickerViewModel viewModel)
            return;

        if (viewModel.SelectedFolders.Count == viewModel.Folders.Count)
        {
            viewModel.SelectedFolders.Clear();
        }
        else
        {
            viewModel.SelectedFolders.Clear();
            foreach (string folder in viewModel.Folders)
            {
                viewModel.SelectedFolders.Add(folder);
            }
        }
    }

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close(Error.Failure(description: "Selection cancelled."));
    }

    private void SelectButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not FoldersPickerViewModel viewModel)
        {
            this.Close(Error.Failure(description: "Couldn't retrieve the view model."));
            return;
        }

        this.Close(viewModel.SelectedFolders.ToList());
    }

    private void SelectedFoldersChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (DataContext is not FoldersPickerViewModel viewModel)
            return;

        SelectAllButton.Content = viewModel.SelectedFolders.Count == viewModel.Folders.Count
            ? "Deselect All"
            : "Select All";

        SelectButton.IsEnabled = viewModel.SelectedFolders.Count > 0;
    }
}