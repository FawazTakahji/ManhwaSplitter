using ErrorOr;

namespace ManhwaSplitter.Core.Services;

public interface IFileService
{
    public Task<ErrorOr<List<string>>> PickImages();
    public Task<ErrorOr<List<string>>> PickFolders();
}