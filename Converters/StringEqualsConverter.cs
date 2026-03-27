using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MonAppMultiplateforme.Converters;

public class StringEqualsConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null || parameter == null)
            return false;

        string valStr = value.ToString() ?? "";
        string paramStr = parameter.ToString() ?? "";

        if (paramStr.StartsWith("!"))
        {
            return valStr != paramStr.Substring(1);
        }

        return valStr == paramStr;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
