using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace MonAppMultiplateforme.Converters;

public class BoolAndConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Count == 0)
            return false;

        foreach (var value in values)
        {
            if (value is bool b && !b)
                return false;
            if (value == null)
                return false;
        }

        return true;
    }
}
