using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using ManhwaSplitter.Core.Services;
using ErrorOr;
using FilePickerFileTypes = ManhwaSplitter.Desktop.Storage.FilePickerFileTypes;
using AvaloniaFilePickerFileTypes = Avalonia.Platform.Storage.FilePickerFileTypes;

namespace ManhwaSplitter.Desktop.Services;

public class FileService : IFileService
{
    public static IStorageProvider? StorageProvider { get; set; }

    public async Task<ErrorOr<List<string>>> PickImages()
    {
        if (StorageProvider is null)
        {
            return Error.Unexpected(description: "Couldn't retrieve the storage provider.");
        }

        IReadOnlyList<IStorageFile> results = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select Images",
            AllowMultiple = true,
            FileTypeFilter = [FilePickerFileTypes.Images, AvaloniaFilePickerFileTypes.All]
        });

        if (results.Count < 1)
            return Error.Failure(description: "No images were selected.");

        return results.Select(file => file.Path.LocalPath)
            .ToList();
    }

    public async Task<ErrorOr<List<string>>> PickFolders()
    {
        if (StorageProvider is null)
        {
            return Error.Unexpected(description: "Couldn't retrieve the storage provider.");
        }

        IReadOnlyList<IStorageFolder> results = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Folders",
            AllowMultiple = true
        });
        if (results.Count < 1)
            return Error.Failure(description: "No folders were selected.");

        return results.Select(folder => folder.Path.LocalPath)
            .ToList();
    }
}