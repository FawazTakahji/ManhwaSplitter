using System.Threading.Tasks;
using Android.OS;
using ErrorOr;
using ManhwaSplitter.Mobile.Android.Helpers;

namespace ManhwaSplitter.Mobile.Android.Services;

public class PermissionServiceApi30 : PermissionServiceBase
{
    public override Task<bool> CheckStoragePermission()
    {
        return Task.FromResult(Environment.IsExternalStorageManager);
    }

    public override async Task<ErrorOr<bool>> RequestStoragePermission()
    {
        if (Environment.IsExternalStorageManager)
            return true;

        return await Storage.RequestManageAllFilesPermission();
    }
}