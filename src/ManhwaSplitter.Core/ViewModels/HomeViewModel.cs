using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManhwaSplitter.Core.Services;
using ErrorOr;
using ManhwaSplitter.Core.Enums;
using ManhwaSplitter.Core.Models;
using ManhwaSplitter.Core.Utilities;
using NLog;
using SixLabors.ImageSharp;

namespace ManhwaSplitter.Core.ViewModels;

public partial class HomeViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool _isSplittingImages;
    [ObservableProperty]
    private int _imagesProgress;
    [ObservableProperty]
    private int _imagesMaxProgress;
    [ObservableProperty]
    private bool _isSplittingFolders;
    [ObservableProperty]
    private int _foldersProgress;
    [ObservableProperty]
    private int _foldersMaxProgress;

    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    public ISettingsService SettingsService { get; }
    private readonly IFileService _fileService;
    private readonly IAppNotificationService _appNotificationService;
    private readonly IScreenService _screenService;
    private readonly IDialogService _dialogService;
    private readonly IPermissionService _permissionService;

    public HomeViewModel(
        ISettingsService settingsService,
        IFileService fileService,
        IAppNotificationService appNotificationService,
        IScreenService screenService,
        IDialogService dialogService,
        IPermissionService permissionService)
    {
        SettingsService = settingsService;
        _fileService = fileService;
        _appNotificationService = appNotificationService;
        _screenService = screenService;
        _dialogService = dialogService;
        _permissionService = permissionService;

        SplitImagesCommand.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SplitImagesCommand.IsRunning))
                NotifyCanExecuteChanged();
        };
        SplitFoldersCommand.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName == nameof(SplitFoldersCommand.IsRunning))
                NotifyCanExecuteChanged();
        };
    }

    [RelayCommand(CanExecute = nameof(CanSplit))]
    private void GetScreenHeight()
    {
        ErrorOr<int> height = _screenService.GetHeight();
        if (height.IsError)
        {
            _appNotificationService.Show("Error", height.FirstError.Description, NotificationType.Error);
            return;
        }

        SettingsService.CurrentSettings.MaxHeight = height.Value;
    }

    [RelayCommand(CanExecute = nameof(CanSplit))]
    private async Task SplitImages(List<string>? files = null)
    {
        if (!await CheckStoragePermission())
            return;

        List<string> filePaths;
        if (files is null)
        {
            ErrorOr<List<string>> pickerResult = await _fileService.PickImages();
            if (pickerResult.IsError)
            {
                _appNotificationService.Show("Selection Failed", pickerResult.FirstError.Description, NotificationType.Error);

                if (pickerResult.FirstError.Metadata?["Exception"] is Exception ex)
                    Logger.Error(ex, "An error occurred while picking images.");

                return;
            }

            filePaths = pickerResult.Value;
        }
        else
        {
            filePaths = files;
        }

        foreach (string path in filePaths)
        {
            ErrorOr<bool> isValid = await Images.IsValid(path);
            if (isValid.IsError)
            {
                _appNotificationService.Show("Selection Failed", isValid.FirstError.Description, NotificationType.Error);

                if (isValid.FirstError.Metadata?["Exception"] is Exception ex)
                    Logger.Error(ex, $"An error occurred while validating the file \"{path}\".");

                return;
            }

            if (!isValid.Value)
            {
                _appNotificationService.Show("Selection Failed", $"\"{Path.GetFileName(path)}\" is not a valid image file.", NotificationType.Error);
                return;
            }
        }

        IsSplittingImages = true;
        ImagesMaxProgress = filePaths.Count;
        int maxHeight = SettingsService.CurrentSettings.MaxHeight;
        CancellationTokenSource cts = new();
        ParallelOptions options = new()
        {
            MaxDegreeOfParallelism = SettingsService.CurrentSettings.SimultaneousOperations,
            CancellationToken = cts.Token
        };

        List<string> oldImagesToDelete = [];
        List<string> croppedImages = [];
        try
        {
            await Parallel.ForEachAsync(filePaths, options, async (imagePath, token) =>
            {
                token.ThrowIfCancellationRequested();

                ErrorOr<(List<string>? CroppedImages, string? OldImage)> result = await SplitImage(imagePath, maxHeight, token);
                if (result.IsError)
                {
                    _appNotificationService.Show($"Split Failed - {Path.GetFileName(imagePath)}", result.FirstError.Description, NotificationType.Error);
                    await cts.CancelAsync();
                }

                if (result.Value.OldImage is not null)
                    oldImagesToDelete.Add(result.Value.OldImage);
                if (result.Value.CroppedImages is not null)
                    croppedImages.AddRange(result.Value.CroppedImages);

                ImagesProgress++;
            });
        }
        catch (OperationCanceledException)
        {
            foreach (string imagePath in croppedImages)
            {
                try
                {
                    File.Delete(imagePath);
                }
                catch (Exception ex)
                {
                    _appNotificationService.Show("Error", $"An error occured while deleting the file \"{Path.GetFileName(imagePath)}\".", NotificationType.Error);
                    Logger.Error(ex, $"An error occurred while deleting the file \"{imagePath}\".");
                }
            }
            return;
        }
        finally
        {
            ImagesMaxProgress = 0;
            ImagesProgress = 0;
            IsSplittingImages = false;
        }

        foreach (string oldImage in oldImagesToDelete)
        {
            try
            {
                File.Delete(oldImage);
            }
            catch (Exception ex)
            {
                _appNotificationService.Show("Error", $"An error occured while deleting the old file \"{Path.GetFileName(oldImage)}\".", NotificationType.Error);
                Logger.Error(ex, $"An error occurred while deleting the old file \"{oldImage}\".");
            }
        }

        _appNotificationService.Show("Split Completed", "The images were split successfully.", NotificationType.Success);
    }

    [RelayCommand(CanExecute = nameof(CanSplit))]
    private async Task SplitFolders(List<string>? folders = null)
    {
        if (!await CheckStoragePermission())
            return;

        List<string> folderPaths;
        if (folders is null)
        {
            ErrorOr<List<string>> pickerResult = await _fileService.PickFolders();
            if (pickerResult.IsError)
            {
                _appNotificationService.Show("Selection Failed", pickerResult.FirstError.Description, NotificationType.Error);

                if (pickerResult.FirstError.Metadata?["Exception"] is Exception ex)
                    Logger.Error(ex, "An error occurred while picking folders.");

                return;
            }

            folderPaths = pickerResult.Value;
        }
        else
        {
            folderPaths = folders;
        }

        IsSplittingFolders = true;
        FoldersMaxProgress = folderPaths.Count;
        int maxHeight = SettingsService.CurrentSettings.MaxHeight;

        List<FailedFolder> failedFolders = [];

        try
        {
            foreach (string folderPath in folderPaths)
            {
                ErrorOr<string[]> files = Directories.GetFiles(folderPath);
                if (files.IsError)
                {
                    _appNotificationService.Show("Split Failed", files.FirstError.Description, NotificationType.Error);
                    if (files.FirstError.Metadata?["Exception"] is Exception ex)
                        Logger.Error(ex, $"An error occurred while getting the files of the folder \"{folderPath}\".");

                    failedFolders.Add(
                        new FailedFolder(Path.GetFileName(folderPath), files.FirstError.Description));
                    continue;
                }

                string[] imageFiles = files.Value
                    .Where(file => Files.IsImageExtension(Path.GetExtension(file)))
                    .ToArray();
                if (imageFiles.Length < 1)
                {
                    _appNotificationService.Show("Split Failed", $"The folder \"{Path.GetFileName(folderPath)}\" doesn't contain any images.", NotificationType.Error);
                    failedFolders.Add(
                        new FailedFolder(Path.GetFileName(folderPath), "The folder doesn't contain any images."));
                    continue;
                }

                IsSplittingImages = true;
                ImagesMaxProgress = imageFiles.Length;
                ParallelOptions options = new()
                {
                    MaxDegreeOfParallelism = SettingsService.CurrentSettings.SimultaneousOperations,
                };

                List<FailedFile> failedFiles = [];
                List<string> oldImagesToDelete = [];

                await Parallel.ForEachAsync(imageFiles, options, async (imagePath, token) =>
                {
                    string fileName = Path.GetFileName(imagePath);
                    ErrorOr<bool> isValid = await Images.IsValid(imagePath);
                    if (isValid.IsError)
                    {
                        _appNotificationService.Show("Split Failed", isValid.FirstError.Description, NotificationType.Error);
                        if (isValid.FirstError.Metadata?["Exception"] is Exception ex)
                            Logger.Error(ex, $"An error occurred while validating the file \"{imagePath}\".");

                        failedFiles.Add(
                            new FailedFile(fileName, isValid.FirstError.Description));
                        return;
                    }
                    if (!isValid.Value)
                    {
                        _appNotificationService.Show("Split Failed", $"\"{Path.GetFileName(imagePath)}\" is not a valid image file.", NotificationType.Error);
                        failedFiles.Add(
                            new FailedFile(fileName, "File is not a valid image."));
                        return;
                    }

                    ErrorOr<(List<string>? CroppedImages, string? OldImage)> result = await SplitImage(imagePath, maxHeight, token);
                    if (result.IsError)
                    {
                        _appNotificationService.Show($"Split Failed - {Path.GetFileName(imagePath)}", result.FirstError.Description, NotificationType.Error);
                        failedFiles.Add(
                            new FailedFile(fileName, result.FirstError.Description));
                        return;
                    }

                    if (result.Value.OldImage is not null)
                        oldImagesToDelete.Add(result.Value.OldImage);

                    ImagesProgress++;
                });

                if (failedFiles.Count > 0)
                {
                    FailedFolder failedFolder = new(Path.GetFileName(folderPath),
                        files: failedFiles.OrderBy(file => file.Name).ToArray());
                    failedFolders.Add(failedFolder);
                }

                IsSplittingImages = false;
                ImagesMaxProgress = 0;
                ImagesProgress = 0;

                foreach (string oldImage in oldImagesToDelete)
                {
                    try
                    {
                        File.Delete(oldImage);
                    }
                    catch (Exception ex)
                    {
                        _appNotificationService.Show("Error", $"An error occured while deleting the old file \"{Path.GetFileName(oldImage)}\".", NotificationType.Error);
                        Logger.Error(ex, $"An error occurred while deleting the old file \"{oldImage}\".");
                    }
                }

                FoldersProgress++;
            }
        }
        finally
        {
            IsSplittingFolders = false;
            FoldersMaxProgress = 0;
            FoldersProgress = 0;
        }

        if (failedFolders.Count > 0)
            await _dialogService.ShowFailedOperations(failedFolders.ToArray());
        else
            _appNotificationService.Show("Split Completed", "The images were split successfully.", NotificationType.Success);
    }

    private async Task<ErrorOr<(List<string>?, string?)>> SplitImage(string imagePath, int maxHeight, CancellationToken token = default)
    {
        ErrorOr<Image> image = await Images.Load(imagePath);
        if (image.IsError)
        {
            if (image.FirstError.Metadata?["Exception"] is Exception ex)
                Logger.Error(ex, $"An error occurred while loading the file \"{imagePath}\".");

            return image.FirstError;
        }
        if (image.Value.Height <= maxHeight)
        {
            image.Value.Dispose();
            return (null, null);
        }

        string? directory = Path.GetDirectoryName(imagePath);
        if (directory is null)
        {
            image.Value.Dispose();
            return Error.Failure(description: $"Failed to get the parent directory for \"{Path.GetFileName(imagePath)}\".");
        }
        string fileName = Path.GetFileNameWithoutExtension(imagePath);
        string extension = Path.GetExtension(imagePath);

        List<Image> images = Images.Split(image.Value, SettingsService.CurrentSettings.MaxHeight);

        try
        {
            ErrorOr<List<string>> savedImages = await Images.Save(images, directory, fileName, extension, token);
            if (savedImages.IsError)
            {
                if (savedImages.FirstError.Metadata?["Exception"] is Exception ex)
                    Logger.Error(ex, $"An error occurred while saving the images to \"{directory}\".");

                return savedImages.FirstError;
            }

            return (savedImages.Value, imagePath);
        }
        finally
        {
            image.Value.Dispose();
            images.ForEach(splitImage => splitImage.Dispose());
        }
    }

    private async Task<bool> CheckStoragePermission()
    {
        if (await _permissionService.CheckStoragePermission())
            return true;

        await _dialogService.Show("Permission Needed", "Please grant the storage permission.", DialogType.Info);
        ErrorOr<bool> result = await _permissionService.RequestStoragePermission();
        if (result.IsError)
        {
            _appNotificationService.Show("Couldn't get the storage permission", result.FirstError.Description, NotificationType.Error);
            return false;
        }
        if (!result.Value)
        {
            _appNotificationService.Show("Split Cancelled", "The user didn't grant the storage permission.", NotificationType.Error);
            return false;
        }

        return true;
    }

    public bool CanSplit() => !SplitImagesCommand.IsRunning && !SplitFoldersCommand.IsRunning;

    private void NotifyCanExecuteChanged()
    {
        GetScreenHeightCommand.NotifyCanExecuteChanged();
        SplitImagesCommand.NotifyCanExecuteChanged();
        SplitFoldersCommand.NotifyCanExecuteChanged();
    }

#if DEBUG
    public HomeViewModel()
    {
        IsSplittingImages = true;
        IsSplittingFolders = true;
    }
#endif
}