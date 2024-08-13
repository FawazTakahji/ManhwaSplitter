using System.Threading.Tasks;
using ErrorOr;
using ManhwaSplitter.Core.Services;

namespace ManhwaSplitter.Mobile.Android.Services;

public class PermissionServiceBase : IPermissionService
{
    public virtual Task<bool> CheckStoragePermission()
    {
        throw new System.NotImplementedException();
    }

    public virtual Task<ErrorOr<bool>> RequestStoragePermission()
    {
        throw new System.NotImplementedException();
    }
}