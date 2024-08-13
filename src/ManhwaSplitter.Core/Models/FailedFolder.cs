namespace ManhwaSplitter.Core.Models;

public class FailedFolder(string name, string? reason = null, FailedFile[]? files = null)
{
    public string Name { get; } = name;
    public string? Reason { get; } = reason;
    public FailedFile[]? Files { get; } = files;
}