using System.Threading.Tasks;
using ErrorOr;
using Microsoft.Maui.ApplicationModel;

namespace ManhwaSplitter.Mobile.Android.Services;

public class PermissionServiceApi23 : PermissionServiceBase
{
    public override async Task<bool> CheckStoragePermission()
    {
        return await Permissions.CheckStatusAsync<Permissions.StorageWrite>() == PermissionStatus.Granted;
    }

    public override async Task<ErrorOr<bool>> RequestStoragePermission()
    {
        return await Permissions.RequestAsync<Permissions.StorageWrite>() == PermissionStatus.Granted;
    }
}