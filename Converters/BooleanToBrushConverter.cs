using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace MonAppMultiplateforme.Converters;

public class BooleanToBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            return b ? Brushes.Green : Brushes.Red;
        }
        
        if (value is string s && bool.TryParse(s, out bool res))
        {
            return res ? Brushes.Green : Brushes.Red;
        }

        return Brushes.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
