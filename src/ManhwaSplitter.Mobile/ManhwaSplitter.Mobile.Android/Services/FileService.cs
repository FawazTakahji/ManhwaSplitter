using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Android.Net;
using Android.Widget;
using Avalonia.Data;
using Avalonia.Platform.Storage;
using ErrorOr;
using ManhwaSplitter.Core.Services;
using ManhwaSplitter.Core.Utilities;
using ManhwaSplitter.Mobile.Android.Helpers;
using ManhwaSplitter.Mobile.Dialogs.ViewModels;
using ManhwaSplitter.Mobile.Dialogs.Views;
using Microsoft.Maui.ApplicationModel;

namespace ManhwaSplitter.Mobile.Android.Services;

public class FileService : IFileService
{
    public async Task<ErrorOr<List<string>>> PickImages()
    {
        if (App.TopLevel?.StorageProvider is null)
            return Error.Unexpected(description: "Couldn't retrieve the storage provider.");

        IReadOnlyList<IStorageFile> results = await App.TopLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Images",
            AllowMultiple = true,
            FileTypeFilter = [FilePickerFileTypes.All]
        });
        if (results.Count < 1)
            return Error.Failure(description: "No images were selected.");

        return GetActualPaths(results);
    }

    public async Task<ErrorOr<List<string>>> PickFolders()
    {
        if (App.TopLevel?.StorageProvider is null)
            return Error.Unexpected(description: "Couldn't retrieve the storage provider.");
        if (Platform.CurrentActivity is null)
            return Error.Unexpected(description: "Couldn't retrieve the current activity.");

        Toast.MakeText(Platform.CurrentActivity, "Select parent folder.", ToastLength.Long)?.Show();
        IReadOnlyList<IStorageFolder> pickerResult = await App.TopLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Folders",
            AllowMultiple = false
        });
        if (pickerResult.Count < 1)
            return Error.Failure(description: "No folder was selected.");

        if (Uri.Parse(pickerResult[0].Path.AbsoluteUri) is not { } uri)
            return Error.Failure(description: $"Couldn't retrieve the path from the uri: \"{pickerResult[0].Path.AbsoluteUri}\".");

        ErrorOr<string> parentFolder = Storage.GetFolderActualPath(uri, Platform.CurrentActivity);
        if (parentFolder.IsError)
            return parentFolder.FirstError;

        ErrorOr<string[]> subDirectories = Directories.GetSubDirectories(parentFolder.Value);
        if (subDirectories.IsError)
            return subDirectories.FirstError;
        if (subDirectories.Value.Length < 1)
            return Error.Failure(description: "Folder doesn't contain any subfolders.");

        FoldersPickerView dialog = new()
        {
            DataContext = new FoldersPickerViewModel(subDirectories.Value.ToList())
        };
        Optional<ErrorOr<List<string>>> dialogResult = await dialog.ShowAsync();

        return dialogResult.Value;
    }

    private static ErrorOr<List<string>> GetActualPaths(IReadOnlyList<IStorageItem> results)
    {
        if (Platform.CurrentActivity is null)
            return Error.Unexpected(description: "Couldn't retrieve the current activity.");

        Uri[] uris = results.Select(result => Uri.Parse(result.Path.AbsoluteUri))
            .OfType<Uri>()
            .ToArray();

        List<string> paths = [];

        foreach (Uri uri in uris)
        {
            ErrorOr<string> result = Storage.GetFileActualPath(uri, Platform.CurrentActivity);

            if (result.IsError)
                return result.FirstError;

            paths.Add(result.Value);
        }

        return paths;
    }
}