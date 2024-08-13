using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;
using ManhwaSplitter.Core.Enums;

namespace ManhwaSplitter.Core.Models;

public partial class Settings : ObservableObject
{
    [ObservableProperty] [property: JsonConverter(typeof(JsonStringEnumConverter))]
    private Theme _theme;
    [ObservableProperty]
    private int _maxHeight = 2340;
    [ObservableProperty]
    private int _simultaneousOperations = 4;

    [ObservableProperty] private bool _checkUpdateOnStartup
#if DEBUG
        ;
#else
        = true;
#endif
}