using System.Threading.Tasks;
using ErrorOr;
using ManhwaSplitter.Core.Services;

namespace ManhwaSplitter.Desktop.Services;

public class PermissionService : IPermissionService
{
    public Task<bool> CheckStoragePermission()
    {
        return Task.FromResult(true);
    }

    public Task<ErrorOr<bool>> RequestStoragePermission()
    {
        ErrorOr<bool> result = true;
        return Task.FromResult(result);
    }
}