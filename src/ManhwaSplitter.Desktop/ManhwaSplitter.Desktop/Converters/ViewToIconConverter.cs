using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;
using ManhwaSplitter.Core.Enums;

namespace ManhwaSplitter.Desktop.Converters;

public class ViewToIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not View view)
            return new BindingNotification(new InvalidCastException("Value is not a View."), BindingErrorType.Error);

        return view switch
        {
            View.Home => "fa-solid fa-home",
            View.Settings => "fa-solid fa-sliders",
            View.About => "fa-solid fa-info-circle",

            _ => "fa-solid fa-question"
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}