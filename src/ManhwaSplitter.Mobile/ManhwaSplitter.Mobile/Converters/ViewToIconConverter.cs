using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using ManhwaSplitter.Core.Enums;
using Material.Icons;

namespace ManhwaSplitter.Mobile.Converters;

public class ViewToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not View view)
            return new BindingNotification(new InvalidCastException("Value is not a View."), BindingErrorType.Error);

        return view switch
        {
            View.Home => MaterialIconKind.Home,
            View.Settings => MaterialIconKind.Slider,
            View.About => MaterialIconKind.Information,

            _ => MaterialIconKind.Help
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}