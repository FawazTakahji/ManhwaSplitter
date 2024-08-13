using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Media;
using Material.Styles.Themes;

namespace ManhwaSplitter.Mobile.Converters;

public class IsSelectedToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool isSelected)
            return new BindingNotification(new InvalidOperationException("The value is not a boolean."), BindingErrorType.Error);

        if (Application.Current is null)
            return new BindingNotification(new InvalidOperationException("Couldn't retrieve the current application instance."), BindingErrorType.Error);

        IReadOnlyTheme currentTheme = Application.Current.LocateMaterialTheme<MaterialTheme>().CurrentTheme;

        return isSelected ? new SolidColorBrush(currentTheme.PrimaryMid.Color) : new SolidColorBrush(currentTheme.Body);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}