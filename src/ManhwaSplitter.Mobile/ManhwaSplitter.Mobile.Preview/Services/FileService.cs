using System.Collections.Generic;
using System.Threading.Tasks;
using ErrorOr;
using ManhwaSplitter.Core.Services;

namespace ManhwaSplitter.Mobile.Preview.Services;

public class FileService : IFileService
{
    public async Task<ErrorOr<List<string>>> PickImages() => Error.Failure();
    public async Task<ErrorOr<List<string>>> PickFolders() => Error.Failure();
}