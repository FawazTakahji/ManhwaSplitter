using ErrorOr;

namespace ManhwaSplitter.Core.Services;

public interface IPermissionService
{
    public Task<bool> CheckStoragePermission();
    public Task<ErrorOr<bool>> RequestStoragePermission();
}