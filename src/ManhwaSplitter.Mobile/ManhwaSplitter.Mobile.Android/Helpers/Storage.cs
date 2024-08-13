using System;
using System.IO;
using ManhwaSplitter.Mobile.Extensions;
using System.Threading.Tasks;
using Android.Content;
using Android.OS;
using Android.Provider;
using ErrorOr;
using Microsoft.Maui.ApplicationModel;
using Uri = Android.Net.Uri;
using Environment = Android.OS.Environment;

namespace ManhwaSplitter.Mobile.Android.Helpers;

public static class Storage
{
    public const int RequestCode = 2296;
    private static TaskCompletionSource<ErrorOr<bool>>? GetPermissionTask { get; set; }

    public static async Task<ErrorOr<bool>> RequestManageAllFilesPermission()
    {
        if (Platform.CurrentActivity is not { } activity)
            return Error.Unexpected(description: "Couldn't retrieve the current activity.");

        if (activity.ApplicationInfo is not {} info)
            return Error.Unexpected(description: "Couldn't retrieve the application info.");

        Uri? uri = Uri.Parse("package:" + info.PackageName);
        if (uri is null)
            return Error.Unexpected(description: "Couldn't retrieve the package URI.");

        try
        {
            GetPermissionTask = new TaskCompletionSource<ErrorOr<bool>>();

            Intent intent = new(Settings.ActionManageAppAllFilesAccessPermission, uri);
            activity.StartActivityForResult(intent, RequestCode);

            return await GetPermissionTask.Task;
        }
        catch (Exception ex)
        {
            return Error.Unexpected(description: "An error occurred while requesting the storage permission.",
                metadata: new()
                {
                    { "Exception", ex }
                });
        }
    }

    public static void OnActivityResult()
    {
        if (Build.VERSION.SdkInt < BuildVersionCodes.R)
            return;

        GetPermissionTask?.SetResult(Environment.IsExternalStorageManager);
    }

    public static ErrorOr<string> GetFileActualPath(Uri uri, Context context)
    {
        if (DocumentsContract.IsDocumentUri(context, uri) && uri.Authority == "com.android.externalstorage.documents")
        {
            string? docId = DocumentsContract.GetDocumentId(uri);
            if (docId != null)
            {
                string[] split = docId.Split(":");
                string type = split[0];

                if (type.Equals("primary", StringComparison.OrdinalIgnoreCase))
                    return Path.Combine(Environment.ExternalStorageDirectory!.Path, split[1]);

                if (IsExternalStorageId(type))
                    return Path.Combine(Environment.StorageDirectory.Path, type, split[1]);
            }
        }

        return Error.Failure(description: $"Couldn't retrieve the path for the file: \"{Path.GetFileName(uri.Path)}\".");
    }

    public static ErrorOr<string> GetFolderActualPath(Uri uri, Context context)
    {
        if (DocumentsContract.BuildDocumentUriUsingTree(uri, DocumentsContract.GetTreeDocumentId(uri)) is { } actualUri
            && DocumentsContract.IsDocumentUri(context, actualUri)
            && actualUri.Authority == "com.android.externalstorage.documents")
        {
            string? docId = DocumentsContract.GetDocumentId(actualUri);
            if (docId != null)
            {
                string[] split = docId.Split(":");
                string type = split[0];

                if (type.Equals("primary", StringComparison.OrdinalIgnoreCase))
                    return Path.Combine(Environment.ExternalStorageDirectory!.Path, split[1]);

                if (IsExternalStorageId(type))
                    return Path.Combine(Environment.StorageDirectory.Path, type, split[1]);
            }
        }

        return Error.Failure(description: $"Couldn't retrieve the path from the uri: \"{uri}\".");
    }

    private static bool IsExternalStorageId(string type)
    {
        // check if string follows pattern of "XXXX-XXXX"
        // X should be a digit or a letter from a-z
        if (type.Length != 9 || type[4] != '-')
            return false;

        return type.CharsAreDigitsOrEnglishLetters(0, 3) && type.CharsAreDigitsOrEnglishLetters(5, 8);
    }
}