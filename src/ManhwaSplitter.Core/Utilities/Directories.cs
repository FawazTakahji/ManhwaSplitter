using ErrorOr;

namespace ManhwaSplitter.Core.Utilities;

public static class Directories
{
    public static ErrorOr<string[]> GetSubDirectories(string directory)
    {
        try
        {
            return Directory.GetDirectories(directory);
        }
        catch (Exception ex)
        {
            return Error.Unexpected(description: $"An error occurred while getting the subfolders of the folder \"{Path.GetFileName(directory)}\".", metadata: new()
            {
                { "Exception", ex }
            });
        }
    }

    public static ErrorOr<string[]> GetFiles(string directory)
    {
        try
        {
            string[] files = Directory.GetFiles(directory);
            return files.Length > 0 ? files : Error.Failure(description: $"The folder \"{Path.GetFileName(directory)}\" is empty.");
        }
        catch (Exception ex)
        {
            return Error.Unexpected(description: $"An error occurred while getting the files of the folder \"{Path.GetFileName(directory)}\".", metadata: new()
            {
                { "Exception", ex }
            });
        }
    }
}