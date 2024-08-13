namespace ManhwaSplitter.Core.Models;

public class FailedFile(string name, string? reason)
{
    public string Name { get; } = name;
    public string? Reason { get; } = reason;
}