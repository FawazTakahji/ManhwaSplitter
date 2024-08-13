using ManhwaSplitter.Core.Enums;

namespace ManhwaSplitter.Core.Models;

public class NavigationItem(string title, View view)
{
    public string Title { get; } = title;
    public View View { get; } = view;
}